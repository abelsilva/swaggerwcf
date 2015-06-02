using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class ExternalDocumentation
    {
        public string Description { get; set; }

        public string Url { get; set; }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }
            if (!string.IsNullOrWhiteSpace(Url))
            {
                writer.WritePropertyName("url");
                writer.WriteValue(Url);
            }

            writer.WriteEndObject();
        }
    }
}
