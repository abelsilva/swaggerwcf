using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using SwaggerWcf.Attributes;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal class Mapper
    {
        internal Mapper(IList<string> hiddenTags, List<string> visibleTags)
        {
            HiddenTags = hiddenTags ?? new List<string>();
            VisibleTags = visibleTags ?? new List<string>();
        }

        internal readonly IEnumerable<string> HiddenTags;
        internal readonly IEnumerable<string> VisibleTags;

        internal IEnumerable<Path> FindMethods(Type markedType, IList<Type> definitionsTypesList, string basePath = null)
        {
            List<Path> paths = new List<Path>();
            List<Tuple<string, PathAction>> pathActions = new List<Tuple<string, PathAction>>();

            List <Type> types;
            Type serviceType;
            if (markedType.IsInterface)
            {
                //search for service impl type
                var allTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(type => markedType.IsAssignableFrom(type) && !type.IsInterface)
                    .ToList();

                serviceType = allTypes.Except(allTypes.Select(type => type.BaseType)).Single();

                types = new List<Type> { markedType };
            }
            else
            {
                serviceType = markedType;

                //search all interfaces for this type for potential DataContracts, and build a set of items
                types = serviceType.GetInterfaces().ToList();
                types.Add(serviceType);
            }

            foreach (Type i in types)
            {
                Attribute dc = i.GetCustomAttribute(typeof(ServiceContractAttribute));
                if (dc == null)
                    continue;

                //found a DataContract, now get a service map and inspect the methods for WebGet/WebInvoke
                if (i.IsInterface)
                {
                    InterfaceMapping map = serviceType.GetInterfaceMap(i);
                    pathActions.AddRange(GetActions(map.TargetMethods, map.InterfaceMethods, definitionsTypesList));

                    //Nested Interface
                    var baseInterfaces = i.GetInterfaces();
                    if (baseInterfaces != null)
                    {
                        foreach (var baseInterface in baseInterfaces)
                        {
                            var _map = serviceType.GetInterfaceMap(baseInterface);
                            pathActions.AddRange(GetActions(_map.TargetMethods, _map.InterfaceMethods, definitionsTypesList));
                        }
                    }
                }
                else
                {
                    pathActions.AddRange(GetActions(i.GetMethods(), i.GetMethods(), definitionsTypesList));
                }
            }

            foreach (var pathAction in pathActions)
            {
                var path = pathAction.Item1;
                if (!path.StartsWith("/"))
                    path = "/" + path;

                if (string.IsNullOrWhiteSpace(basePath) == false)
                    path = basePath + path;

                GetPath(path, paths).Actions.Add(pathAction.Item2);
            }

            return paths;
        }

        private Path GetPath(string id, List<Path> paths)
        {
            var path = paths.FirstOrDefault(p => p.Id == id);
            if (path == null)
            {
                path = new Path
                {
                    Id = id,
                    Actions = new List<PathAction>()
                };
                paths.Add(path);
            }

            return path;
        }

        private static string ConcatPaths(string basePath, string pathUrl)
        {
            string path = basePath;
            if (basePath.EndsWith("/") && pathUrl.StartsWith("/"))
                path += pathUrl.Substring(1);
            else if (!basePath.EndsWith("/") && !string.IsNullOrWhiteSpace(pathUrl) && !pathUrl.StartsWith("/"))
                path += "/" + pathUrl;
            else
                path += pathUrl;
            return path;
        }

        internal IEnumerable<Tuple<string, PathAction>> GetActions(MethodInfo[] targetMethods,
                                                                   MethodInfo[] interfaceMethods,
                                                                   IList<Type> definitionsTypesList)
        {
            int methodsCounts = interfaceMethods.Count();
            for (int index = 0; index < methodsCounts; index++)
            {
                MethodInfo implementation = targetMethods[index];
                MethodInfo declaration = interfaceMethods[index];

                //if a tag from either implementation or declaration is marked as not visible, skip it
                List<SwaggerWcfTagAttribute> methodTags =
                    implementation.GetCustomAttributes<SwaggerWcfTagAttribute>().ToList();
                methodTags =
                    methodTags.Concat(declaration.GetCustomAttributes<SwaggerWcfTagAttribute>()).ToList();

                methodTags = methodTags.Distinct().ToList();

                if (methodTags.Select(t => t.TagName).Any(HiddenTags.Contains))
                    continue;

                //if the method is marked Hidden anywhere, skip it
                if ((implementation.GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null ||
                     declaration.GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null) &&
                    !methodTags.Select(t => t.TagName).Any(VisibleTags.Contains))
                    continue;

                //find the WebGet/Invoke attributes, or skip if neither is present
                WebGetAttribute wg = declaration.GetCustomAttribute<WebGetAttribute>();
                WebInvokeAttribute wi = declaration.GetCustomAttribute<WebInvokeAttribute>();
                if (wg == null && wi == null)
                    continue;

                string httpMethod = (wi == null) ? "GET" : wi.Method ?? "POST";
                string uriTemplate = GetUriTemplate(wi, wg, declaration);

                bool wrappedRequest = IsRequestWrapped(wg, wi);
                bool wrappedResponse = IsResponseWrapped(wg, wi);

                //implementation description overrides interface description
                string description =
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(implementation, "Description") ??
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(declaration, "Description") ??
                    Helpers.GetCustomAttributeValue<string, DescriptionAttribute>(implementation, "Description") ??
                    Helpers.GetCustomAttributeValue<string, DescriptionAttribute>(declaration, "Description") ??
                    "";

                string summary =
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(implementation, "Summary") ??
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(declaration, "Summary") ??
                    "";

                string operationId =
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(implementation, "OperationId") ??
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(declaration, "OperationId") ??
                    "";
                if (operationId == "")
                {
                    if (implementation.DeclaringType != null)
                        operationId = implementation.DeclaringType.FullName + ".";
                    operationId += implementation.Name;
                }

                string externalDocsDescription =
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(implementation,
                                                                                     "ExternalDocsDescription") ??
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(declaration,
                                                                                     "ExternalDocsDescription") ??
                    "";

                string externalDocsUrl =
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(implementation, "ExternalDocsUrl") ??
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(declaration, "ExternalDocsUrl") ??
                    "";

                ExternalDocumentation externalDocs = null;
                if (!string.IsNullOrWhiteSpace(externalDocsDescription) || !string.IsNullOrWhiteSpace(externalDocsUrl))
                {
                    externalDocs = new ExternalDocumentation
                    {
                        Description = externalDocsDescription,
                        Url = HttpUtility.HtmlEncode(externalDocsUrl)
                    };
                }

                bool deprecated =
                    Helpers.GetCustomAttributeValue<SwaggerWcfPathAttribute>(implementation, "Deprecated");
                if (!deprecated)
                    deprecated = Helpers.GetCustomAttributeValue<SwaggerWcfPathAttribute>(declaration, "Deprecated");

                string operationPath =
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(implementation, "OperationPath") ??
                    Helpers.GetCustomAttributeValue<string, SwaggerWcfPathAttribute>(declaration, "OperationPath");
                if (!string.IsNullOrWhiteSpace(operationPath))
                {
                    uriTemplate = ConcatPaths(operationPath, uriTemplate);
                }

                PathAction operation = new PathAction
                {
                    Id = httpMethod.ToLowerInvariant(),
                    Summary = summary,
                    Description = description,
                    Tags =
                        methodTags.Where(t => !t.HideFromSpec).Select(t => HttpUtility.HtmlEncode(t.TagName)).ToList(),
                    Consumes = new List<string>(GetConsumes(implementation, declaration)),
                    Produces = new List<string>(GetProduces(implementation, declaration)),
                    Deprecated = deprecated,
                    OperationId = HttpUtility.HtmlEncode(operationId),
                    ExternalDocs = externalDocs,
                    Responses = GetResponseCodes(implementation, declaration, wrappedResponse, definitionsTypesList),
                    Security = GetMethodSecurity(implementation, declaration)
                    // Schemes = TODO: how to get available schemes for this WCF service? (schemes: http/https)
                };

                //try to map each implementation parameter to the uriTemplate.
                ParameterInfo[] parameters = declaration.GetParameters();
                if (parameters.Any())
                    operation.Parameters = new List<ParameterBase>();

                List<SwaggerWcfHeaderAttribute> headers =
                    implementation.GetCustomAttributes<SwaggerWcfHeaderAttribute>().ToList();
                headers = headers.Concat(declaration.GetCustomAttributes<SwaggerWcfHeaderAttribute>()).ToList();

                // remove duplicates
                headers = headers.GroupBy(h => h.Name).Select(g => g.First()).ToList();

                // parameters - headers
                foreach (SwaggerWcfHeaderAttribute attr in headers)
                {
                    operation.Parameters.Add(new ParameterPrimitive
                    {
                        Name = attr.Name,
                        Description = attr.Description,
                        Default = attr.DefaultValue,
                        In = InType.Header,
                        Required = attr.Required,
                        TypeFormat = new TypeFormat(ParameterType.String, null)
                    });
                }

                bool isGetRequest = httpMethod == "GET";
                int bodyParameterCount = parameters.Where(p => GetInType(uriTemplate, p.Name) == InType.Body).Count();
                TypeBuilder typeBuilder = null;
                if (!wrappedRequest && !isGetRequest && bodyParameterCount > 1)
                {
                    wrappedRequest = true;
                }
                if (wrappedRequest)
                {
                    typeBuilder = new TypeBuilder(implementation.GetWrappedName(declaration));
                }

                var declarationName = declaration.Name;
                foreach (ParameterInfo parameter in parameters)
                {
                    SwaggerWcfParameterAttribute settings =
                        implementation.GetParameters()
                            .First(p => p.Position.Equals(parameter.Position))
                            .GetCustomAttribute<SwaggerWcfParameterAttribute>() ??
                        parameter.GetCustomAttribute<SwaggerWcfParameterAttribute>();

                    if (implementation.GetParameters()
                            .First(p => p.Position.Equals(parameter.Position))
                            .GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null ||
                        parameter.GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null)
                        continue;

                    List<SwaggerWcfTagAttribute> piTags =
                        methodTags.Concat(parameter.GetCustomAttributes<SwaggerWcfTagAttribute>())
                            .ToList();

                    InType inType = GetInType(uriTemplate, parameter.Name);

                    if (inType == InType.Body && wrappedRequest)
                    {
                        bool required = settings != null && settings.Required;

                        if (!required && !parameter.HasDefaultValue)
                            required = true;

                        typeBuilder.AddField(parameter.Name, parameter.ParameterType, required);

                        continue;
                    }

                    if (piTags.Select(t => t.TagName).Any(HiddenTags.Contains))
                        continue;

                    Type type = settings == null || settings.ParameterType == null
                        ? parameter.ParameterType
                        : settings.ParameterType;

                    TypeFormat typeFormat = Helpers.MapSwaggerType(type, definitionsTypesList);

                    operation.Parameters.Add(GetParameter(typeFormat, declaration, implementation, parameter, settings, uriTemplate, wrappedRequest,
                                                          definitionsTypesList, inType));
                }

                if (wrappedRequest)
                {
                    TypeFormat typeFormat = Helpers.MapSwaggerType(typeBuilder.Type, definitionsTypesList);

                    operation.Parameters.Add(new ParameterSchema
                    {
                        Name = implementation.GetWrappedName(declaration) + "Wrapper",
                        In = InType.Body,
                        Required = true,
                        SchemaRef = typeFormat.Format
                    });
                }

                if (!string.IsNullOrWhiteSpace(uriTemplate))
                {
                    int indexOfQuestionMark = uriTemplate.IndexOf('?');
                    if (indexOfQuestionMark >= 0)
                        uriTemplate = uriTemplate.Substring(0, indexOfQuestionMark);

                    uriTemplate = RemoveParametersDefaultValuesFromUri(uriTemplate);
                }
                yield return new Tuple<string, PathAction>(uriTemplate, operation);
            }
        }

        private string GetUriTemplate(WebInvokeAttribute wi, WebGetAttribute wg, MethodInfo declaration)
        {
            return ((wi == null) ? wg.UriTemplate : wi.UriTemplate) ?? declaration.Name;
        }

        private string RemoveParametersDefaultValuesFromUri(string uriTemplate)
        {
            Regex regWithDefaultValue = new Regex(@"\{[a-zA-Z0-9_]+(=[a-zA-Z0-9_]+)\}");
            if (!regWithDefaultValue.Match(uriTemplate).Success)
                return uriTemplate;

            string res = uriTemplate;
            int currIndex = 0;
            Match match;
            while ((match = regWithDefaultValue.Match(res)).Success)
            {
                res = res.Substring(currIndex, match.Groups[1].Index) +
                      res.Substring(match.Groups[1].Index + match.Groups[1].Length);
            }

            return res;
        }

        private static bool IsRequestWrapped(WebGetAttribute wg, WebInvokeAttribute wi)
        {
            return (wg != null) &&
                   (wg.BodyStyle == WebMessageBodyStyle.Wrapped || wg.BodyStyle == WebMessageBodyStyle.WrappedRequest)
                   ||
                   (wi != null) &&
                   (wi.BodyStyle == WebMessageBodyStyle.Wrapped || wi.BodyStyle == WebMessageBodyStyle.WrappedRequest);
        }

        private static bool IsResponseWrapped(WebGetAttribute wg, WebInvokeAttribute wi)
        {
            return (wg != null) &&
                   (wg.BodyStyle == WebMessageBodyStyle.Wrapped || wg.BodyStyle == WebMessageBodyStyle.WrappedResponse)
                   ||
                   (wi != null) &&
                   (wi.BodyStyle == WebMessageBodyStyle.Wrapped || wi.BodyStyle == WebMessageBodyStyle.WrappedResponse);
        }

        private ParameterBase GetParameter(TypeFormat typeFormat,
                                           MethodInfo declaration,
                                           MethodInfo implementation,
                                           ParameterInfo parameter,
                                           SwaggerWcfParameterAttribute settings,
                                           string uriTemplate,
                                           bool wrappedRequest,
                                           IList<Type> definitionsTypesList,
                                           InType inType)
        {
            string description = settings?.Description;
            bool required = settings != null && settings.Required;
            string name = inType == InType.Query ? ResolveParameterNameFromUri(uriTemplate, parameter) : parameter.Name;

            if (inType == InType.Path)
                required = true;

            if (!required && !parameter.HasDefaultValue)
                required = true;

            Type paramType = settings == null || settings.ParameterType == null
                ? parameter.ParameterType
                : settings.ParameterType;
            if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                required = false;
                paramType = paramType.GenericTypeArguments[0];
            }

            if (typeFormat.Type == ParameterType.Object)
            {
                return new ParameterSchema
                {
                    Name = name,
                    Description = description,
                    In = inType,
                    Required = required,
                    SchemaRef = typeFormat.Format
                };
            }

            if (inType == InType.Body)
            {
                if (typeFormat.Type == ParameterType.Array)
                {
                    Type t = paramType.GetElementType() ?? GetEnumerableType(paramType);
                    TypeFormat subTypeFormat = Helpers.MapSwaggerType(t);

                    ParameterItems items;

                    if (subTypeFormat.IsPrimitiveType || subTypeFormat.IsEnum)
                    {
                        items = new ParameterItems
                        {
                            TypeFormat = subTypeFormat
                        };
                    }
                    else
                    {
                        items = new ParameterItems
                        {
                            Items = new ParameterSchema
                            {

                                SchemaRef = t.GetModelName()
                            }
                        };
                    }

                    ParameterPrimitive arrayParam = new ParameterPrimitive
                    {
                        Name = name,
                        Description = description,
                        In = inType,
                        Required = required,
                        TypeFormat = typeFormat,
                        Items = items,
                        CollectionFormat = CollectionFormat.Csv
                    };

                    //it's a complex type, so we'll need to map it later
                    if (definitionsTypesList != null && !definitionsTypesList.Contains(t))
                    {
                        definitionsTypesList.Add(t);
                    }

                    return arrayParam;
                }
                if (typeFormat.IsPrimitiveType || typeFormat.IsEnum)
                {
                    bool isGetRequest = implementation.GetCustomAttributes<WebGetAttribute>().Any() ||
                                        declaration.GetCustomAttributes<WebGetAttribute>().Any();
                    if (!isGetRequest)
                    {
                        WebInvokeAttribute webInvoke = implementation.GetCustomAttribute<WebInvokeAttribute>()
                                                       ?? declaration.GetCustomAttribute<WebInvokeAttribute>();
                        if (webInvoke != null && webInvoke.Method == "GET")
                            isGetRequest = true;
                    }
                    if (!wrappedRequest && isGetRequest)
                    {
                        ParameterPrimitive paramPrimitive = new ParameterPrimitive
                        {
                            Name = name,
                            Description = description,
                            In = InType.Query,
                            Required = required,
                            TypeFormat = typeFormat
                        };
                        return paramPrimitive;
                    }
                    else
                    {
                        ParameterPrimitive paramPrimitive = new ParameterPrimitive
                        {
                            Name = name,
                            Description = description,
                            In = inType,
                            Required = required,
                            TypeFormat = typeFormat
                        };
                        return paramPrimitive;
                    }
                }

                //it's a complex type, so we'll need to map it later
                if (definitionsTypesList != null && !definitionsTypesList.Contains(paramType))
                {
                    definitionsTypesList.Add(paramType);
                }

                typeFormat = new TypeFormat(ParameterType.Object,
                                             HttpUtility.HtmlEncode(paramType.GetModelName()));

                return new ParameterSchema
                {
                    Name = name,
                    Description = description,
                    In = inType,
                    Required = required,
                    SchemaRef = typeFormat.Format
                };
            }

            ParameterPrimitive param = new ParameterPrimitive
            {
                Name = name,
                Description = description,
                In = inType,
                Required = required,
                TypeFormat = typeFormat
            };

            return param;
        }

        private static string ResolveParameterNameFromUri(string uriTemplate, ParameterInfo parameter)
        {
            int questionMarkPosition = uriTemplate.IndexOf("?", StringComparison.Ordinal);
            var uriParameters = HttpUtility.ParseQueryString(uriTemplate.Substring(questionMarkPosition + 1));

            string parameterTemplate = GetParameterNameTemplate(parameter.Name);

            var resolvedParameter = uriParameters
                .AllKeys
                .Select(k => new { Name = k, Template = uriParameters.Get(k) })
                .FirstOrDefault(p => p.Template.Equals(parameterTemplate, StringComparison.Ordinal));

            return resolvedParameter != null ? resolvedParameter.Name : parameter.Name;
        }

        private static string GetParameterNameTemplate(string parameterName)
        {
            return $"{{{parameterName}}}";
        }

        private InType GetInType(string uriTemplate, string parameterName)
        {
            Match match = new Regex(GetParameterNameTemplate(parameterName)).Match(uriTemplate);

            if (!match.Success)
                return InType.Body;

            int questionMarkPosition = uriTemplate.IndexOf("?", StringComparison.Ordinal);

            if (questionMarkPosition == -1)
                return InType.Path;

            return questionMarkPosition > match.Index ? InType.Path : InType.Query;
        }

        private IEnumerable<string> GetConsumes(MethodInfo implementation, MethodInfo declaration)
        {
            string[] overrides = implementation
                .GetCustomAttributes<SwaggerWcfContentTypesAttribute>()
                .Concat(declaration.GetCustomAttributes<SwaggerWcfContentTypesAttribute>())
                .Select(attr => attr.ConsumeTypes)
                .FirstOrDefault(types => types != null && types.Length > 0);

            if (overrides != null)
            {
                return overrides;
            }

            List<string> contentTypes = new List<string>();
            if (declaration.GetCustomAttributes<WebGetAttribute>().Any(a => a.IsRequestFormatSetExplicitly))
            {
                contentTypes.AddRange(
                    declaration.GetCustomAttributes<WebGetAttribute>()
                        .Select(a => ConvertWebMessageFormatToContentType(a.RequestFormat)));
            }
            else if (declaration.GetCustomAttributes<WebInvokeAttribute>().Any(a => a.IsRequestFormatSetExplicitly))
            {
                contentTypes.AddRange(
                    declaration.GetCustomAttributes<WebInvokeAttribute>()
                        .Select(a => ConvertWebMessageFormatToContentType(a.RequestFormat)));
            }
            if (!contentTypes.Any())
                contentTypes.AddRange(new[] { "application/json", "application/xml" });

            return contentTypes;
        }

        private IEnumerable<string> GetProduces(MethodInfo implementation, MethodInfo declaration)
        {
            string[] overrides = implementation.GetCustomAttributes<SwaggerWcfContentTypesAttribute>()
                .Concat(declaration.GetCustomAttributes<SwaggerWcfContentTypesAttribute>())
                .Select(attr => attr.ProduceTypes)
                .FirstOrDefault(types => types != null && types.Length > 0);

            if (overrides != null)
            {
                return overrides;
            }

            List<string> contentTypes = new List<string>();
            if (declaration.GetCustomAttributes<WebGetAttribute>().Any(a => a.IsResponseFormatSetExplicitly))
            {
                contentTypes.AddRange(
                    declaration.GetCustomAttributes<WebGetAttribute>()
                        .Select(a => ConvertWebMessageFormatToContentType(a.ResponseFormat)));
            }
            else if (declaration.GetCustomAttributes<WebInvokeAttribute>().Any(a => a.IsResponseFormatSetExplicitly))
            {
                contentTypes.AddRange(
                    declaration.GetCustomAttributes<WebInvokeAttribute>()
                        .Select(a => ConvertWebMessageFormatToContentType(a.ResponseFormat)));
            }
            if (!contentTypes.Any())
                contentTypes.AddRange(new[] { "application/json", "application/xml" });

            return contentTypes;
        }

        private string ConvertWebMessageFormatToContentType(WebMessageFormat format)
        {
            switch (format)
            {
                case WebMessageFormat.Xml:
                    return "application/xml";
                case WebMessageFormat.Json:
                default:
                    return "application/json";
            }
        }

        private List<Response> GetResponseCodes(MethodInfo implementation, MethodInfo declaration, bool wrappedResponse,
                                                IList<Type> definitionsTypesList)
        {
            Type returnType = implementation.GetCustomAttributes<SwaggerWcfReturnTypeAttribute>()
                                  .Concat(declaration.GetCustomAttributes<SwaggerWcfReturnTypeAttribute>())
                                  .Select(attr => attr.ReturnType)
                                  .FirstOrDefault()
                              ?? declaration.ReturnType;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                returnType = returnType.GenericTypeArguments[0];

            Schema schema = returnType.IsEnum
                                ? BuildSchemaForEnum(returnType, definitionsTypesList)
                                : BuildSchema(returnType, implementation, declaration, wrappedResponse, definitionsTypesList);

            List<SwaggerWcfResponseAttribute> responses =
                implementation.GetCustomAttributes<SwaggerWcfResponseAttribute>().ToList();
            responses = responses.Concat(declaration.GetCustomAttributes<SwaggerWcfResponseAttribute>()).FilterUnique().ToList();

            List<Response> res =
                responses.Select(ra => ConvertResponse(ra, schema, implementation, declaration, wrappedResponse, definitionsTypesList))
                    .ToList();

            if (!res.Any())
            {
                res.Add(new Response
                {
                    Code = "default",
                    Schema = schema
                });
            }

            return res;
        }

        private Response ConvertResponse(SwaggerWcfResponseAttribute ra, Schema schema, MethodInfo implementation, MethodInfo declaration,
                                         bool wrappedResponse, IList<Type> definitionsTypesList)
        {
            Schema s = schema;

            if (ra.EmptyResponseOverride)
                s = null;
            else if (ra.ResponseTypeOverride != null)
                s = BuildSchema(ra.ResponseTypeOverride, implementation, declaration, wrappedResponse, definitionsTypesList);
            else if (schema != null && schema.TypeFormat.Type == ParameterType.Array)
            {
                Type type = schema.Ref != null ? Type.GetType(schema.Ref) : null;
                if (type != null)
                {
                    TypeFormat arrayTypeFormat = Helpers.MapSwaggerType(type);
                    if (arrayTypeFormat.IsPrimitiveType)
                    {
                        schema.ArrayTypeFormat = arrayTypeFormat;
                    }
                }
            }
            return new Response
            {
                Code = ra.Code,
                Description = ra.Description,
                Schema = s,
                Headers = (ra.Headers != null) ? ra.Headers.ToList() : null,
                Example = GetExample(ra)
            };
        }

        private Example GetExample(SwaggerWcfResponseAttribute ra)
        {
            if (string.IsNullOrWhiteSpace(ra.ExampleContent))
                return null;

            return new Example
            {
                MimeType = ra.ExampleMimeType,
                Content = ra.ExampleContent
            };
        }

        private Schema BuildSchemaForEnum(Type returnType, IList<Type> definitionsTypesList)
        {
            //it's a complex type, so we'll need to map it later
            if (definitionsTypesList != null && !definitionsTypesList.Contains(returnType))
            {
                definitionsTypesList.Add(returnType);
            }
            TypeFormat typeFormat = new TypeFormat(ParameterType.Unknown, null);

            return new Schema
            {
                TypeFormat = typeFormat,
                Ref = HttpUtility.HtmlEncode(returnType.GetModelName())
            };
        }

        private Schema BuildSchema(Type type, MethodInfo implementation, MethodInfo declaration, bool wrappedResponse, IList<Type> definitionsTypesList)
        {
            if (type == typeof(void) || type == typeof(Task))
                return null;

            if (type.BaseType == typeof(Task))
                type = GetTaskInnerType(type);

            TypeFormat typeFormat;
            if (wrappedResponse)
            {
                string funcName = implementation.Name;

                string typeName = implementation.GetCustomAttribute<SwaggerWcfReturnTypeAttribute>()?.Name
                                  ?? declaration.GetCustomAttribute<SwaggerWcfReturnTypeAttribute>()?.Name
                                  ?? funcName + "Result";

                TypeBuilder typeBuilder = new TypeBuilder(typeName + "Wrapper");

                typeBuilder.AddField(typeName, type, true);

                typeFormat = Helpers.MapSwaggerType(typeBuilder.Type, definitionsTypesList);
            }
            else
            {
                typeFormat = Helpers.MapSwaggerType(type, definitionsTypesList);
            }

            switch (typeFormat.Type)
            {
                case ParameterType.Object:
                    return new Schema
                    {
                        Ref = typeFormat.Format
                    };
                case ParameterType.Array:
                    Type t = type.GetElementType() ?? GetEnumerableType(type);
                    if (t == null)
                        return null;

                    TypeFormat arrayTypeFormat = Helpers.MapSwaggerType(t);
                    if (arrayTypeFormat.IsPrimitiveType)
                    {
                        return new Schema
                        {
                            TypeFormat = typeFormat,
                            ArrayTypeFormat = arrayTypeFormat
                        };
                    }
                    else
                    {
                        definitionsTypesList.Add(t);
                        return new Schema
                        {
                            TypeFormat = typeFormat,
                            Ref = HttpUtility.HtmlEncode(t.GetModelName())
                        };
                    }
                default:
                    definitionsTypesList.Add(type);
                    return new Schema
                    {
                        TypeFormat = typeFormat
                    };
            }
        }

        private static object GetDefaultValue(Type type)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Type GetTaskInnerType(Type type)
        {
            if (type == null)
                return null;

            return type.BaseType == typeof(Task) ? type.GetGenericArguments()[0] : type;
        }

        private static List<KeyValuePair<string, string[]>> GetMethodSecurity(MethodInfo implementation, MethodInfo declaration)
        {
            var securityMethodAttributes = implementation.GetCustomAttributes<SwaggerWcfSecurityAttribute>().ToList();
            var securityInterfaceAttributes = declaration.GetCustomAttributes<SwaggerWcfSecurityAttribute>().ToList();

            var securityAttributes = securityMethodAttributes.Concat(securityInterfaceAttributes).ToList();

            if (securityAttributes.Any() == false)
                return null;

            var security = new List<KeyValuePair<string, string[]>>();

            foreach (var securityAttribute in securityAttributes)
            {
                security.Add(new KeyValuePair<string, string[]>(securityAttribute.SecurityDefinitionName, securityAttribute.SecurityDefinitionScopes));
            }


            return security;
        }

        public static Type GetEnumerableType(Type type)
        {
            if (type == null)
                return null;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            Type iface = (from i in type.GetInterfaces()
                          where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                          select i).FirstOrDefault();

            return iface == null ? null : GetEnumerableType(iface);
        }
    }
}
