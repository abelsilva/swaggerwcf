using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    public class InfoLicense
    {
        public string Name { get; set; }

        public string Url { get; set; }

        internal void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(Name);
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
