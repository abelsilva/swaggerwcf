using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class Definition
    {
        public Schema Schema { get; set; }

        public void Serialize(JsonWriter writer)
        {
            Schema.Serialize(writer);
        }
    }
}
