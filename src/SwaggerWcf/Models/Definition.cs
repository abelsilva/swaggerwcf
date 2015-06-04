using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
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
