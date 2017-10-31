using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class ParameterItems
    {
        public TypeFormat TypeFormat { get; set; }

        public ParameterBase Items { get; set; }
        
        public string Ref { get; set; }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (TypeFormat.Type != ParameterType.Unknown)
            {
                writer.WritePropertyName("type");
                writer.WriteValue(TypeFormat.Type.ToString().ToLower());
                if (!string.IsNullOrWhiteSpace(TypeFormat.Format))
                {
                    writer.WritePropertyName("format");
                    writer.WriteValue(TypeFormat.Format);
                }
            }
            
            if (!string.IsNullOrWhiteSpace(Ref))
            {
                writer.WritePropertyName("$ref");
                writer.WriteValue($"#/definitions/{Ref}");
            }

            Items?.Serialize(writer, true);

            writer.WriteEndObject();
        }
    }
}
