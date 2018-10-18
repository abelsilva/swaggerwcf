using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal static class TypePropertiesProcessor
    {

        public static void ProcessProperties(Type definitionType, DefinitionSchema schema, IList<string> hiddenTags,
                                              Stack<Type> typesStack)
        {
            PropertyInfo[] properties = definitionType.GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                DefinitionProperty prop = ProcessProperty(propertyInfo, hiddenTags, typesStack);

                if (prop == null)
                    continue;

                if (prop.TypeFormat.Type == ParameterType.Array)
                {
                    Type propType = propertyInfo.PropertyType;

                    Type t = propType.GetElementType() ?? DefinitionsBuilder.GetEnumerableType(propType);

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

            SwaggerWcfRegexAttribute regexAttr = propertyInfo.GetCustomAttribute<SwaggerWcfRegexAttribute>();
            if (regexAttr != null)
                prop.Pattern = regexAttr.Regex;

            prop.TypeFormat = typeFormat;

            if (prop.TypeFormat.Type == ParameterType.Object)
            {
                typesStack.Push(propertyInfo.PropertyType);

                prop.Ref = propertyInfo.PropertyType.GetModelName();

                return prop;
            }

            if (prop.TypeFormat.Type == ParameterType.Array)
            {
                Type subType = DefinitionsBuilder.GetEnumerableType(propertyInfo.PropertyType);
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

            if ((prop.TypeFormat.Type == ParameterType.Integer && prop.TypeFormat.Format == "enum") || (prop.TypeFormat.Type == ParameterType.Array && prop.Items.TypeFormat.Format == "enum"))
            {
                prop.Enum = new List<int>();

                Type propType = propertyInfo.PropertyType;

                if (propType.IsGenericType && (propType.GetGenericTypeDefinition() == typeof(Nullable<>) || propType.GetGenericTypeDefinition() == typeof(List<>)))
                    propType = propType.GetEnumerableType();

                string enumDescription = "";
                List<string> listOfEnumNames = propType.GetEnumNames().ToList();
                foreach (string enumName in listOfEnumNames)
                {
                    var enumMemberItem = Enum.Parse(propType, enumName, true);
                    string enumMemberDescription = DefinitionsBuilder.GetEnumDescription((Enum)enumMemberItem);
                    enumMemberDescription = (string.IsNullOrWhiteSpace(enumMemberDescription)) ? "" : $"({enumMemberDescription})";
                    int enumMemberValue = DefinitionsBuilder.GetEnumMemberValue(propType, enumName);
                    if (prop.Description != null) prop.Enum.Add(enumMemberValue);
                    enumDescription += $"    {enumName}{System.Web.HttpUtility.HtmlEncode(" = ")}{enumMemberValue} {enumMemberDescription}\r\n";
                }

                if (enumDescription != "")
                {
                    prop.Description += $"\r\n\r\n{enumDescription}";
                }
            }

            // Apply any options set in a [SwaggerWcfProperty]
            DefinitionsBuilder.ApplyAttributeOptions(propertyInfo, prop);

            return prop;
        }

    }
}
