using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwaggerWcf.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Path
    {
        public string Id { get; set; }

        public List<PathAction> Actions { get; set; }

        public Path()
        {
            Actions = new List<PathAction>();
        }

        public void Serialize(JsonWriter writer)
        {
            writer.WritePropertyName(Id);

            writer.WriteStartObject();

            foreach (PathAction pathAction in Actions)
            {
                pathAction.Serialize(writer);
            }

            writer.WriteEndObject();
        }
    }
}
