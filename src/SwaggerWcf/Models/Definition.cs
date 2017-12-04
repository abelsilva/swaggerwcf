using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwaggerWcf.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    internal class Definition
    {
        public DefinitionSchema Schema { get; set; }

        public void Serialize(JsonWriter writer)
        {
            if (Schema == null)
                return;

            writer.WritePropertyName(Schema.Name);

            writer.WriteStartObject();
            Schema.Serialize(writer);
            writer.WriteEndObject();
        }
    }
}
