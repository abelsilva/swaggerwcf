using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using SwaggerWcf.Attributes;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal sealed class DefinitionsBuilder
    {
        public static List<Definition> Process(IList<string> hiddenTags, List<Type> definitionsTypes)
        {
            var definitions = new List<Definition>();
            var typesStack = new Stack<Type>(definitionsTypes.GroupBy(t => t.FullName).Select(grp => grp.First()));

            while (typesStack.Any())
            {
                Type t = typesStack.Pop();
                if (IsHidden(t, hiddenTags))
                    continue;

                definitions.Add(ConvertTypeToDefinition(t, hiddenTags, typesStack));
            }

            return definitions;
            //TODO: think about inheritance
            //only pass immediate class properties at a time to write properties in the order of inheritance from base. (i.e. base first, derived next.) 
            //get a stack of class types within the passed type so that the base class comes at the top.
            //var classStack = new Stack<Type>();
            //foreach (
            //    PropertyInfo propertyInfo in
            //        properties.Where(propertyInfo => !classStack.Contains(propertyInfo.DeclaringType)))
            //{
            //    classStack.Push(propertyInfo.DeclaringType);
            //}
            // to get properties only from current class:
            //IEnumerable<PropertyInfo> propertiesToWrite = classType.properties.Where(p => p.DeclaringType == classType)
        }

        private static bool IsHidden(Type type, IList<string> hiddenTags)
        {
            if (hiddenTags.Contains(type.FullName))
                return true;

            if (type.GetCustomAttribute<HiddenAttribute>() != null)
                return true;

            if (type.GetCustomAttributes<TagAttribute>().Select(t => t.TagName).Any(hiddenTags.Contains))
                return true;

            return false;
        }

        private static Definition ConvertTypeToDefinition(Type definitionType, IList<string> hiddenTags, Stack<Type> typesStack)
        {
            var schema = new DefinitionSchema
            {
                Name = definitionType.FullName
            };

            ProcessTypeAttributes(definitionType, schema);

            // process
            schema.TypeFormat = Helpers.MapSwaggerType(definitionType, null);
            ProcessProperties(definitionType, schema, hiddenTags, typesStack);

            return new Definition
            {
                Schema = schema
            };
        }

        private static void ProcessTypeAttributes(Type definitionType, DefinitionSchema schema)
        {
            var descAttr = definitionType.GetCustomAttribute<DescriptionAttribute>();
            if (descAttr != null)
                schema.Description = descAttr.Description;

            var definitionAttr = definitionType.GetCustomAttribute<DefinitionAttribute>();
            if (definitionAttr != null)
            {
                if (!string.IsNullOrWhiteSpace(definitionAttr.Description))
                    schema.Description = definitionAttr.Description;
                if (!string.IsNullOrWhiteSpace(definitionAttr.ExternalDocsDescription) ||
                    !string.IsNullOrWhiteSpace(definitionAttr.ExternalDocsUrl))
                {
                    schema.ExternalDocumentation = new ExternalDocumentation
                    {
                        Description = definitionAttr.ExternalDocsDescription,
                        Url = definitionAttr.ExternalDocsUrl
                    };
                }
            }
        }

        private static void ProcessProperties(Type definitionType, DefinitionSchema schema, IList<string> hiddenTags, Stack<Type> typesStack)
        {
            PropertyInfo[] properties = definitionType.GetProperties();
            schema.Properties = new List<DefinitionProperty>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                DefinitionProperty prop = ProcessProperty(propertyInfo, hiddenTags, typesStack);

                if (prop == null)
                    continue;

                if (prop.Required)
                    schema.Required.Add(prop.Title);

                schema.Properties.Add(prop);
            }
        }

        private static DefinitionProperty ProcessProperty(PropertyInfo propertyInfo, IList<string> hiddenTags, Stack<Type> typesStack)
        {
            if (propertyInfo.GetCustomAttribute<DataMemberAttribute>() == null
                || propertyInfo.GetCustomAttribute<HiddenAttribute>() != null
                || propertyInfo.GetCustomAttributes<TagAttribute>().Select(t => t.TagName).Any(hiddenTags.Contains))
                return null;

            var prop = new DefinitionProperty { Title = propertyInfo.Name };

            var dataMemberAttribute = propertyInfo.GetCustomAttribute<DataMemberAttribute>();
            if (dataMemberAttribute != null)
            {
                if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                    prop.Title = dataMemberAttribute.Name;

                prop.Required = dataMemberAttribute.IsRequired;
            }

            var descriptionAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                prop.Description = descriptionAttribute.Description;

            prop.TypeFormat = Helpers.MapSwaggerType(propertyInfo.PropertyType, null);

            if (prop.TypeFormat.Type == ParameterType.Array)
            {
                Type subType = propertyInfo.PropertyType.GetEnumerableType();
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


            //TODO > arrays and enums
            //if (Helpers.MapSwaggerType(pType, _definitions.Select(d => d.Type).ToList()) == "array")
            //{
            //    writer.WritePropertyName("items");
            //    writer.WriteStartObject();
            //    writer.WritePropertyName("$ref");
            //    writer.WriteValue(Helpers.MapElementType(pType, _definitions.Select(d => d.Type).ToList()));
            //}

            //if (pType.IsEnum)
            //{
            //    writer.WritePropertyName("enum");
            //    writer.WriteStartArray();
            //    foreach (string value in pType.GetEnumNames())
            //    {
            //        writer.WriteValue(value);
            //    }
            //    writer.WriteEndArray();
            //}

            return prop;
        }
    }
}
