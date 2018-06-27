using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class Response
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public Schema Schema { get; set; }

        public List<string> Headers { get; set; }

        public Example Example { get; set; }

        public void Serialize(JsonWriter writer)
        {
            writer.WritePropertyName(Code);

            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }
            else
            {
                writer.WritePropertyName("description");
                writer.WriteValue("not available");
            }

            if (Schema != null)
            {
                writer.WritePropertyName("schema");
                writer.WriteStartObject();
                Schema.Serialize(writer);
                writer.WriteEndObject();
            }

            if (Example != null)
            {
                writer.WritePropertyName("examples");
                writer.WriteStartObject();
                writer.WritePropertyName(Example.MimeType);
                writer.WriteRawValue(Example.Content);
                writer.WriteEndObject();
            }

            if (Headers != null && Headers.Any())
            {
                writer.WritePropertyName("headers");

                writer.WriteStartObject();

                foreach (var header in Headers)
                {
                    writer.WritePropertyName(header);
                    writer.WriteStartObject();
                    writer.WritePropertyName("type");
                    writer.WriteValue("string");
                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
            }
            
            writer.WriteEndObject();
        }
    }
}
