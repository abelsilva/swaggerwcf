using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using SwaggerWcf.Attributes;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal class Mapper
    {
        internal Mapper(IList<string> hiddenTags)
        {
            HiddenTags = hiddenTags ?? new List<string>();
        }

        internal readonly IEnumerable<string> HiddenTags;

        internal IEnumerable<Path> FindMethods(string basePath, Type serviceType, IList<Type> definitionsTypesList)
        {
            bool addedSlash = false;
            List<Path> paths = new List<Path>();
            List<Tuple<string, PathAction>> pathActions = new List<Tuple<string, PathAction>>();

            if (!basePath.EndsWith("/"))
            {
                addedSlash = true;
                basePath = basePath + "/";
            }

            //search all interfaces for this type for potential DataContracts, and build a set of items
            List<Type> types = serviceType.GetInterfaces().ToList();
            types.Add(serviceType);
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
                }
                else
                {
                    pathActions.AddRange(GetActions(i.GetMethods(), i.GetMethods(), definitionsTypesList));
                }
            }

            foreach (Tuple<string, PathAction> pathAction in pathActions)
            {
                string path = basePath;
                if (string.IsNullOrWhiteSpace(pathAction.Item1) && addedSlash)
                    path = path.Substring(0, path.Length - 1);

                GetPath(path, pathAction.Item1, paths).Actions.Add(pathAction.Item2);
            }

            return paths;
        }

        private Path GetPath(string basePath, string pathUrl, List<Path> paths)
        {
            string id = basePath;
            if (basePath.EndsWith("/") && pathUrl.StartsWith("/"))
                id += pathUrl.Substring(1);
            else if (!basePath.EndsWith("/") && !string.IsNullOrWhiteSpace(pathUrl) && !pathUrl.StartsWith("/"))
                id += "/" + pathUrl;
            else
                id += pathUrl;

            Path path = paths.FirstOrDefault(p => p.Id == id);
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

        internal IEnumerable<Tuple<string, PathAction>> GetActions(MethodInfo[] targetMethods,
                                                                   MethodInfo[] interfaceMethods,
                                                                   IList<Type> definitionsTypesList)
        {
            int methodsCounts = interfaceMethods.Count();
            for (int index = 0; index < methodsCounts; index++)
            {
                MethodInfo implementation = targetMethods[index];
                MethodInfo declaration = interfaceMethods[index];

                //if the method is marked Hidden anywhere, skip it
                if (implementation.GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null ||
                    declaration.GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null)
                    continue;

                //if a tag from either implementation or declaration is marked as not visible, skip it
                List<SwaggerWcfTagAttribute> methodTags =
                    implementation.GetCustomAttributes<SwaggerWcfTagAttribute>().ToList();
                methodTags =
                    methodTags.Concat(declaration.GetCustomAttributes<SwaggerWcfTagAttribute>()).ToList();
                methodTags = methodTags.Distinct().ToList();

                if (methodTags.Select(t => t.TagName).Any(HiddenTags.Contains))
                    continue;

                //find the WebGet/Invoke attributes, or skip if neither is present
                WebGetAttribute wg = declaration.GetCustomAttribute<WebGetAttribute>();
                WebInvokeAttribute wi = declaration.GetCustomAttribute<WebInvokeAttribute>();
                if (wg == null && wi == null)
                    continue;

                string httpMethod = (wi == null) ? "GET" : wi.Method ?? "POST";
                string uriTemplate = (wi == null) ? (wg.UriTemplate ?? "") : (wi.UriTemplate ?? "");

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
                        Description = HttpUtility.HtmlEncode(externalDocsDescription),
                        Url = HttpUtility.HtmlEncode(externalDocsUrl)
                    };
                }

                bool deprecated =
                    Helpers.GetCustomAttributeValue<SwaggerWcfPathAttribute>(implementation, "Deprecated");
                if (!deprecated)
                    deprecated = Helpers.GetCustomAttributeValue<SwaggerWcfPathAttribute>(declaration, "Deprecated");

                PathAction operation = new PathAction
                {
                    Id = httpMethod.ToLowerInvariant(),
                    Summary = HttpUtility.HtmlEncode(summary),
                    Description = HttpUtility.HtmlEncode(description),
                    Tags =
                        methodTags.Where(t => !t.HideFromSpec).Select(t => HttpUtility.HtmlEncode(t.TagName)).ToList(),
                    Consumes = new List<string>(GetConsumes(implementation, declaration)),
                    Produces = new List<string>(GetProduces(implementation, declaration)),
                    Deprecated = deprecated,
                    OperationId = HttpUtility.HtmlEncode(operationId),
                    ExternalDocs = externalDocs,
                    Responses = GetResponseCodes(implementation, declaration, definitionsTypesList)
                    // Schemes = TODO: how to get available schemes for this WCF service? (schemes: http/https)
                };

                //try to map each implementation parameter to the uriTemplate.
                ParameterInfo[] parameters = declaration.GetParameters();
                if (parameters.Any())
                    operation.Parameters = new List<ParameterBase>();
                foreach (ParameterInfo parameter in parameters)
                {
                    TypeFormat typeFormat = Helpers.MapSwaggerType(parameter.ParameterType, definitionsTypesList);

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

                    if (piTags.Select(t => t.TagName).Any(HiddenTags.Contains))
                        continue;

                    operation.Parameters.Add(GetParameter(typeFormat, parameter, settings, uriTemplate,
                        definitionsTypesList));
                }

                if (!string.IsNullOrWhiteSpace(uriTemplate))
                {
                    int indexOfQuestionMark = uriTemplate.IndexOf('?');
                    if (indexOfQuestionMark >= 0)
                        uriTemplate = uriTemplate.Substring(0, indexOfQuestionMark);
                }
                yield return new Tuple<string, PathAction>(uriTemplate, operation);
            }
        }

        private ParameterBase GetParameter(TypeFormat typeFormat, ParameterInfo parameter,
                                           SwaggerWcfParameterAttribute settings, string uriTemplate,
                                           IList<Type> definitionsTypesList)
        {
            string description = settings != null ? settings.Description : null;
            bool required = settings != null && settings.Required;
            string name = parameter.Name;
            DataMemberAttribute dataMemberAttribute = parameter.GetCustomAttribute<DataMemberAttribute>();

            if (dataMemberAttribute != null && !string.IsNullOrEmpty(dataMemberAttribute.Name))
                name = dataMemberAttribute.Name;

            InType inType = GetInType(uriTemplate, parameter.Name);
            if (inType == InType.Path)
                required = true;

            if (!required && !parameter.HasDefaultValue)
                required = true;

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
                    Type t = parameter.ParameterType.GetElementType() ?? GetEnumerableType(parameter.ParameterType);
                    ParameterPrimitive arrayParam = new ParameterPrimitive
                    {
                        Name = name,
                        Description = description,
                        In = inType,
                        Required = required,
                        TypeFormat = typeFormat,
                        Items = new ParameterItems
                        {
                            Items = new ParameterSchema
                            {
                                SchemaRef = t.FullName
                            }
                        },
                        CollectionFormat = CollectionFormat.Csv
                    };

                    //it's a complex type, so we'll need to map it later
                    if (definitionsTypesList != null && !definitionsTypesList.Contains(t))
                    {
                        definitionsTypesList.Add(t);
                    }

                    return arrayParam;
                }

                //it's a complex type, so we'll need to map it later
                if (definitionsTypesList != null && !definitionsTypesList.Contains(parameter.ParameterType))
                {
                    definitionsTypesList.Add(parameter.ParameterType);
                }
                typeFormat = new TypeFormat(ParameterType.Object,
                    HttpUtility.HtmlEncode(parameter.ParameterType.FullName));

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

        private InType GetInType(string uriTemplate, string parameterName)
        {
            if (!uriTemplate.Contains(parameterName))
                return InType.Body;

            int questionMarkPosition = uriTemplate.IndexOf("?", StringComparison.Ordinal);

            if (questionMarkPosition == -1)
                return InType.Path;

            return (questionMarkPosition > uriTemplate.IndexOf(parameterName, StringComparison.Ordinal))
                ? InType.Path
                : InType.Query;
        }

        private IEnumerable<string> GetConsumes(MethodInfo implementation, MethodInfo declaration)
        {
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
                contentTypes.AddRange(new[] {"application/json", "application/xml"});

            return contentTypes;
        }

        private IEnumerable<string> GetProduces(MethodInfo implementation, MethodInfo declaration)
        {
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
                contentTypes.AddRange(new[] {"application/json", "application/xml"});

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

        private List<Response> GetResponseCodes(MethodInfo implementation, MethodInfo declaration,
                                                IList<Type> definitionsTypesList)
        {
            Schema schema = BuildSchema(declaration.ReturnType, definitionsTypesList);

            List<SwaggerWcfResponseAttribute> responses =
                implementation.GetCustomAttributes<SwaggerWcfResponseAttribute>().ToList();
            responses = responses.Concat(declaration.GetCustomAttributes<SwaggerWcfResponseAttribute>()).ToList();

            List<Response> res = responses.Select(ra => new Response
            {
                Code = ra.Code,
                Description = ra.Description,
                Schema = ra.EmptyResponseOverride ? null : schema,
                Headers = (ra.Headers != null) ? ra.Headers.ToList() : null
            }).ToList();

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

        private Schema BuildSchema(Type type, IList<Type> definitionsTypesList)
        {
            if (type == typeof(void))
                return null;

            if (type.BaseType == typeof(System.Threading.Tasks.Task))
                type = GetTaskInnerType(type);

            TypeFormat typeFormat = Helpers.MapSwaggerType(type, definitionsTypesList);

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
                    definitionsTypesList.Add(t);
                    return new Schema
                    {
                        TypeFormat = typeFormat,
                        Ref = HttpUtility.HtmlEncode(t.FullName)
                    };
                default:
                    definitionsTypesList.Add(type);
                    return new Schema
                    {
                        TypeFormat = typeFormat
                    };
            }
        }

        private static Type GetTaskInnerType(Type type)
        {
            if (type == null)
                return null;

            return type.BaseType == typeof(System.Threading.Tasks.Task) ? type.GetGenericArguments()[0] : type;
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
