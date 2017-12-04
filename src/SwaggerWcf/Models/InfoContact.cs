using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwaggerWcf.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class InfoContact
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Url { get; set; }

        internal void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(Name);
            }
            if (!string.IsNullOrWhiteSpace(Email))
            {
                writer.WritePropertyName("email");
                writer.WriteValue(Email);
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
