using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class DefinitionSchema
    {
        public TypeFormat TypeFormat { get; set; } // for primitives

        public string Ref { get; set; } // for references

        public string Name { get; set; }

        public string XmlName { get; set; }

        public string XmlNamespace { get; set; }

        public string Description { get; set; }

        public Schema ParentSchema { get; set; } //TODO: Composition and Polymorphism Support

        public ExternalDocumentation ExternalDocumentation { get; set; }

        public List<string> Required { get; set; }

        public List<DefinitionProperty> Properties { get; set; }

        public List<int> Enum { get; set; }

        public void Serialize(JsonWriter writer)
        {
            if (TypeFormat.Type == ParameterType.Object)
            {
                // complex object

                if (!string.IsNullOrWhiteSpace(Description))
                {
                    writer.WritePropertyName("description");
                    writer.WriteValue(Description);
                }

                if (ParentSchema != null)
                {
                    writer.WritePropertyName("allOf");

                    writer.WriteStartArray();

                    writer.WritePropertyName("$ref");
                    writer.WriteValue(string.Format("#/definitions/{0}", ParentSchema.Name));

                    writer.WriteStartObject();
                }

                SerializeRequired(writer);

                SerializeExternalDocs(writer);

                SerializeProperties(writer);

                if (ParentSchema != null)
                {
                    writer.WriteEndObject();

                    writer.WriteEndArray();
                }
            }
            if (TypeFormat.Type != ParameterType.Unknown)
            {
                writer.WritePropertyName("type");
                writer.WriteValue(TypeFormat.Type.ToString().ToLower());
                if (!string.IsNullOrWhiteSpace(TypeFormat.Format))
                {
                    writer.WritePropertyName("format");
                    writer.WriteValue(TypeFormat.Format);
                }

                if (TypeFormat.Type == ParameterType.Array)
                {
                    writer.WritePropertyName("items");

                    writer.WriteStartObject();

                    writer.WritePropertyName("$ref");
                    writer.WriteValue(string.Format("#/definitions/{0}", Ref));

                    writer.WriteEndObject();
                }
                if (Enum != null && Enum.Any())
                {
                    writer.WritePropertyName("enum");
                    writer.WriteStartArray();
                    foreach (int e in Enum)
                    {
                        writer.WriteValue(e);
                    }
                    writer.WriteEndArray();
                }
            }
            else if (!string.IsNullOrWhiteSpace(Ref))
            {
                writer.WritePropertyName("$ref");
                writer.WriteValue(string.Format("#/definitions/{0}", Ref));
            }

            SerializeXmlProperties(writer);
        }

        private void SerializeRequired(JsonWriter writer)
        {
            if (Required != null && Required.Any())
            {
                writer.WritePropertyName("required");
                writer.WriteStartArray();
                foreach (string req in Required)
                {
                    writer.WriteValue(req);
                }
                writer.WriteEndArray();
            }
        }

        private void SerializeExternalDocs(JsonWriter writer)
        {
            if (ExternalDocumentation?.Url != null)
            {
                writer.WritePropertyName("externalDocs");
                ExternalDocumentation.Serialize(writer);
            }
        }

        private void SerializeProperties(JsonWriter writer)
        {
            if (Properties != null && Properties.Any())
            {
                writer.WritePropertyName("properties");
                writer.WriteStartObject();
                foreach (DefinitionProperty p in Properties)
                {
                    p.Serialize(writer);
                }
                writer.WriteEndObject();
            }
        }

        private void SerializeXmlProperties(JsonWriter writer)
        {
            writer.WritePropertyName("xml");
            writer.WriteStartObject();
            if (string.IsNullOrEmpty(XmlName))
            {
                var strs = Name.Split('.');
                var str = strs[strs.Length - 1];
                writer.WritePropertyName("name");
                writer.WriteValue(str);
            }
            else
            {
                writer.WritePropertyName("name");
                writer.WriteValue(XmlName);
            }

            if (XmlNamespace == string.Empty)
            {
                writer.WritePropertyName("namespace");
                writer.WriteValue(string.Empty);
            }
            else if (XmlNamespace == null)
            {
                string defaultNS = "http://schemas.datacontract.org/2004/07";
                int index = Name.LastIndexOf('.');
                if (index > 0)
                {
                    defaultNS += "/" + Name.Substring(0, index);
                }
                writer.WritePropertyName("namespace");
                writer.WriteValue(defaultNS);
            }
            else
            {
                writer.WritePropertyName("namespace");
                writer.WriteValue(XmlNamespace);
            }

            writer.WriteEndObject();
        }
    }
}
