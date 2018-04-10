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

            if (schema.TypeFormat.Type == ParameterType.Integer && schema.TypeFormat.Format == "enum")
            {
                schema.Enum = new List<int>();

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
                schema.Properties = new List<DefinitionProperty>();
                TypePropertiesProcessor.ProcessProperties(definitionType, schema, hiddenTags, typesStack);
                TypeFieldsProcessor.ProcessFields(definitionType, schema, hiddenTags, typesStack);
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

        public static void ApplyAttributeOptions(PropertyInfo propertyInfo, DefinitionProperty prop)
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
            ApplyAttributeOptions(attrs, prop);
        }

        public static void ApplyAttributeOptions(FieldInfo fieldInfo, DefinitionProperty prop)
        {
            // Use the DataContract [DefaultValue] as the default, by default
            var defAttr = fieldInfo.GetCustomAttributes<DefaultValueAttribute>().LastOrDefault();
            if (defAttr != null)
            {
                prop.Default = defAttr.Value.ToString();
            }

            // Apply any [SwaggerWcfProperty]s in order.
            var attrs = fieldInfo.GetCustomAttributes<SwaggerWcfPropertyAttribute>().ToList();
            if (!attrs.Any())
            {
                return;
            }
            ApplyAttributeOptions(attrs, prop);
        }

        public static void ApplyAttributeOptions(IEnumerable<SwaggerWcfPropertyAttribute> attrs, DefinitionProperty prop)
        {
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
        
        public static int GetEnumMemberValue(Type enumType, string enumName)
        {
            if (string.IsNullOrWhiteSpace(enumName))
                return 0;
            var enumVal = Enum.Parse(enumType, enumName, true);
            var underlyingType = Enum.GetUnderlyingType(enumType);
            var val = Convert.ChangeType(enumVal, underlyingType);
            return Convert.ToInt32(val);
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return "";
        }
    }
}
