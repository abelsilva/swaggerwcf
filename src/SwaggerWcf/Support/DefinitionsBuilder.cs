using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal sealed class DefinitionsBuilder
    {
        public static List<Definition> Process(IList<string> hiddenTags, IList<string> visibleTags, List<Type> definitionsTypes)
        {
            if (definitionsTypes == null || !definitionsTypes.Any())
                return new List<Definition>(0);

            List<Definition> definitions = new List<Definition>();
            List<Type> processedTypes = new List<Type>();
            Stack<Type> typesStack =
                new Stack<Type>(definitionsTypes.GroupBy(t => t.FullName).Select(grp => grp.First()));

            while (typesStack.Any())
            {
                Type t = typesStack.Pop();
                if (IsHidden(t, hiddenTags, visibleTags) || processedTypes.Contains(t))
                    continue;

                processedTypes.Add(t);
                Definition definition = ConvertTypeToDefinition(t, hiddenTags, typesStack);
                if (definition != null)
                    definitions.Add(definition);
            }

            return definitions;
        }

        private static bool IsHidden(Type type, ICollection<string> hiddenTags, ICollection<string> visibleTags)
        {
            if (hiddenTags.Contains(type.FullName))
                return true;

            if (type.GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null)
            {
                return !type.GetCustomAttributes<SwaggerWcfTagAttribute>().Select(t => t.TagName).Any(visibleTags.Contains);
            }

            return type.GetCustomAttributes<SwaggerWcfTagAttribute>().Select(t => t.TagName).Any(hiddenTags.Contains);
        }

        private static Definition ConvertTypeToDefinition(Type definitionType, IList<string> hiddenTags,
                                                          Stack<Type> typesStack)
        {
            DefinitionSchema schema = new DefinitionSchema
            {
                Name = definitionType.GetModelName()
            };

            ProcessTypeAttributes(definitionType, schema);

            // process
            schema.TypeFormat = Helpers.MapSwaggerType(definitionType, null);

            if (schema.TypeFormat.IsPrimitiveType)
                return null;

            if (schema.TypeFormat.Type == ParameterType.String && schema.TypeFormat.Format == "enum")
            {
                schema.Enum = new List<string>();

                Type propType = definitionType;

                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propType = propType.GetEnumerableType();

                List<string> listOfEnumNames = propType.GetEnumNames().ToList();
                foreach (string enumName in listOfEnumNames)
                {
                    schema.Enum.Add(GetEnumMemberValue(propType, enumName));
                }
            }
            else if (schema.TypeFormat.Type == ParameterType.Array)
            {
                Type t = GetEnumerableType(definitionType);

                if (t != null)
                {
                    schema.Ref = definitionType.GetModelName();
                    typesStack.Push(t);
                }
            }
            else
            {
                ProcessProperties(definitionType, schema, hiddenTags, typesStack);
            }

            return new Definition
            {
                Schema = schema
            };
        }

        private static void ProcessTypeAttributes(Type definitionType, DefinitionSchema schema)
        {
            DescriptionAttribute descAttr = definitionType.GetCustomAttribute<DescriptionAttribute>();
            if (descAttr != null)
                schema.Description = descAttr.Description;

            SwaggerWcfDefinitionAttribute definitionAttr =
                definitionType.GetCustomAttribute<SwaggerWcfDefinitionAttribute>();
            if (definitionAttr != null)
            {
                if (!string.IsNullOrWhiteSpace(definitionAttr.ExternalDocsDescription) ||
                    !string.IsNullOrWhiteSpace(definitionAttr.ExternalDocsUrl))
                {
                    schema.ExternalDocumentation = new ExternalDocumentation
                    {
                        Description = definitionAttr.ExternalDocsDescription,
                        Url = definitionAttr.ExternalDocsUrl
                    };
                }

                if (!string.IsNullOrWhiteSpace(definitionAttr.ModelName))
                    schema.Name = definitionAttr.ModelName;
            }
        }

        private static void ProcessProperties(Type definitionType, DefinitionSchema schema, IList<string> hiddenTags,
                                              Stack<Type> typesStack)
        {
            PropertyInfo[] properties = definitionType.GetProperties();
            schema.Properties = new List<DefinitionProperty>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                DefinitionProperty prop = ProcessProperty(propertyInfo, hiddenTags, typesStack);

                if (prop == null)
                    continue;

                if (prop.TypeFormat.Type == ParameterType.Array)
                {
                    Type propType = propertyInfo.PropertyType;

                    Type t = propType.GetElementType() ?? GetEnumerableType(propType);

                    if (t != null)
                    {
                        //prop.TypeFormat = new TypeFormat(prop.TypeFormat.Type, HttpUtility.HtmlEncode(t.FullName));
                        prop.TypeFormat = new TypeFormat(prop.TypeFormat.Type, null);

                        TypeFormat st = Helpers.MapSwaggerType(t);
                        if (st.Type == ParameterType.Array || st.Type == ParameterType.Object)
                        {
                            prop.Items.TypeFormat = new TypeFormat(ParameterType.Unknown, null);
                            prop.Items.Ref = t.GetModelName();
                        }
                        else
                        {
                            prop.Items.TypeFormat = st;
                        }
                    }
                }

                if (prop.Required)
                {
                    if (schema.Required == null)
                        schema.Required = new List<string>();

                    schema.Required.Add(prop.Title);
                }
                schema.Properties.Add(prop);
            }
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

        private static DefinitionProperty ProcessProperty(PropertyInfo propertyInfo, IList<string> hiddenTags,
                                                          Stack<Type> typesStack)
        {
            if (propertyInfo.GetCustomAttribute<SwaggerWcfHiddenAttribute>() != null
                || propertyInfo.GetCustomAttributes<SwaggerWcfTagAttribute>()
                               .Select(t => t.TagName)
                               .Any(hiddenTags.Contains))
                return null;

            TypeFormat typeFormat = Helpers.MapSwaggerType(propertyInfo.PropertyType, null);

            DefinitionProperty prop = new DefinitionProperty { Title = propertyInfo.Name };

            DataMemberAttribute dataMemberAttribute = propertyInfo.GetCustomAttribute<DataMemberAttribute>();
            if (dataMemberAttribute != null)
            {
                if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                    prop.Title = dataMemberAttribute.Name;

                prop.Required = dataMemberAttribute.IsRequired;
            }

            // Special case - if it came out required, but we unwrapped a null-able type,
            // then it's necessarily not required.  Ideally this would only set the default,
            // but we can't tell the difference between an explicit declaration of
            // IsRequired =false on the DataMember attribute and no declaration at all.
            if (prop.Required && propertyInfo.PropertyType.IsGenericType &&
                propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                prop.Required = false;
            }

            DescriptionAttribute descriptionAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                prop.Description = descriptionAttribute.Description;

            prop.TypeFormat = typeFormat;

            if (prop.TypeFormat.Type == ParameterType.Object)
            {
                typesStack.Push(propertyInfo.PropertyType);

                prop.Ref = propertyInfo.PropertyType.GetModelName();

                return prop;
            }

            if (prop.TypeFormat.Type == ParameterType.Array)
            {
                Type subType = GetEnumerableType(propertyInfo.PropertyType);
                if (subType != null)
                {
                    TypeFormat subTypeFormat = Helpers.MapSwaggerType(subType, null);

                    if (subTypeFormat.Type == ParameterType.Object)
                        typesStack.Push(subType);

                    prop.Items = new ParameterItems
                    {
                        TypeFormat = subTypeFormat
                    };
                }
            }

            if (prop.TypeFormat.Type == ParameterType.String && prop.TypeFormat.Format == "enum")
            {
                prop.Enum = new List<string>();

                Type propType = propertyInfo.PropertyType;

                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propType = propType.GetEnumerableType();

                List<string> listOfEnumNames = propType.GetEnumNames().ToList();
                foreach (string enumName in listOfEnumNames)
                {
                    prop.Enum.Add(GetEnumMemberValue(propType, enumName));
                }
            }

            // Apply any options set in a [SwaggerWcfProperty]
            ApplyAttributeOptions(propertyInfo, prop);

            return prop;
        }

        private static T LastValidValue<T>(IEnumerable<SwaggerWcfPropertyAttribute> attrs,
                                           Func<SwaggerWcfPropertyAttribute, T> getter)
        {
            return attrs.Select(getter).LastOrDefault(x => x != null);
        }

        private static void ApplyIfValid<T>(T opt, Action<T> setter)
        {
            if (opt != null)
            {
                setter(opt);
            }
        }

        private static void ApplyAttributeOptions(PropertyInfo propertyInfo, DefinitionProperty prop)
        {
            // Use the DataContract [DefaultValue] as the default, by default
            var defAttr = propertyInfo.GetCustomAttributes<DefaultValueAttribute>().LastOrDefault();
            if (defAttr != null)
            {
                prop.Default = defAttr.Value.ToString();
            }

            // Apply any [SwaggerWcfProperty]s in order.
            var attrs = propertyInfo.GetCustomAttributes<SwaggerWcfPropertyAttribute>().ToList();
            if (!attrs.Any())
            {
                return;
            }

            ApplyIfValid(LastValidValue(attrs, a => a.Title), x => prop.Title = x);
            ApplyIfValid(LastValidValue(attrs, a => a.Description), x => prop.Description = x);
            ApplyIfValid(LastValidValue(attrs, a => a._Required), x => prop.Required = x.Value);
            //ApplyIfValid(LastValidValue(attrs, a => a._AllowEmptyValue),  x => prop.AllowEmptyValue  = x.Value);
            //ApplyIfValid(LastValidValue(attrs, a => a._CollectionFormat), x => prop.CollectionFormat = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a.Default), x => prop.Default = x);
            ApplyIfValid(LastValidValue(attrs, a => a.Example), x => prop.Example = x);
            ApplyIfValid(LastValidValue(attrs, a => a._Maximum), x => prop.Maximum = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._ExclusiveMaximum), x => prop.ExclusiveMaximum = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._Minimum), x => prop.Minimum = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._ExclusiveMinimum), x => prop.ExclusiveMinimum = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._MaxLength), x => prop.MaxLength = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._MinLength), x => prop.MinLength = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a.Pattern), x => prop.Pattern = x);
            ApplyIfValid(LastValidValue(attrs, a => a._MaxItems), x => prop.MaxItems = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._MinItems), x => prop.MinItems = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._UniqueItems), x => prop.UniqueItems = x.Value);
            ApplyIfValid(LastValidValue(attrs, a => a._MultipleOf), x => prop.MultipleOf = x.Value);
        }

        private static string GetEnumMemberValue(Type enumType, string enumName)
        {
            if (string.IsNullOrWhiteSpace(enumName))
                return null;
            var enumVal = Enum.Parse(enumType, enumName, true);
            var underlyingType = Enum.GetUnderlyingType(enumType);
            var value = Convert.ChangeType(enumVal, underlyingType);
            return Convert.ToString(value);
        }
    }
}
