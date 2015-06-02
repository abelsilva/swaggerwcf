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
            IEnumerable<Type> visibleDefinitions = definitionsTypes.Where(t => IsVisible(t, hiddenTags));
            return
                visibleDefinitions.GroupBy(t => t.FullName)
                                  .Select(grp => ConvertTypeToDefinition(grp.First(), hiddenTags))
                                  .ToList();

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

        private static bool IsVisible(Type type, IList<string> hiddenTags)
        {
            if (hiddenTags.Contains(type.FullName))
                return false;

            if (type.GetCustomAttribute<HiddenAttribute>() != null)
                return false;

            if (type.GetCustomAttributes<TagAttribute>().Select(t => t.TagName).Any(hiddenTags.Contains))
                return false;

            return true;
        }

        private static Definition ConvertTypeToDefinition(Type definitionType, IList<string> hiddenTags)
        {
            var schema = new Schema
            {
                Name = definitionType.FullName
            };

            ProcessTypeAttributes(definitionType, schema);

            // process
            schema.TypeFormat = Helpers.MapSwaggerType(definitionType, null);
            ProcessProperties(definitionType, schema, hiddenTags);

            return new Definition
            {
                Schema = schema
            };
        }

        private static void ProcessTypeAttributes(Type definitionType, Schema schema)
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

        private static void ProcessProperties(Type definitionType, Schema schema, IList<string> hiddenTags)
        {
            PropertyInfo[] properties = definitionType.GetProperties();
            schema.Properties = new List<Property>();

            foreach (PropertyInfo propertyInfo in properties)
            {
                Property prop = ProcessProperty(propertyInfo, hiddenTags);

                if (prop == null)
                    continue;

                if (prop.Required)
                    schema.Required.Add(prop.Title);

                schema.Properties.Add(prop);
            }
        }

        private static Property ProcessProperty(PropertyInfo propertyInfo, IList<string> hiddenTags)
        {
            if (propertyInfo.GetCustomAttribute<DataMemberAttribute>() == null
                || propertyInfo.GetCustomAttribute<HiddenAttribute>() != null
                || propertyInfo.GetCustomAttributes<TagAttribute>().Select(t => t.TagName).Any(hiddenTags.Contains))
                return null;

            var prop = new Property { Title = propertyInfo.Name };

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
