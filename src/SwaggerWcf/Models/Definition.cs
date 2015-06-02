using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class Definition
    {
        public Schema Schema { get; set; }

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
