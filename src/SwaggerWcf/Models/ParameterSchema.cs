using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class ParameterSchema : ParameterBase
    {
        public string SchemaRef { get; set; }

        public override void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(Name);
            }
            if (!string.IsNullOrWhiteSpace(In))
            {
                writer.WritePropertyName("in");
                writer.WriteValue(In);
            }
            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }
            writer.WritePropertyName("required");
            writer.WriteValue(Required);

            if (!string.IsNullOrWhiteSpace(SchemaRef))
            {
                writer.WritePropertyName("schema");
                writer.WriteStartObject();

                writer.WritePropertyName("$ref");
                writer.WriteValue(string.Format("#/definitions/{0}", SchemaRef));

                writer.WriteEndObject();
            }
            
            writer.WriteEndObject();
        }
    }
}
