using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class Path
    {
        public Path()
        {
            Actions = new List<PathAction>();
        }

        public string Id { get; set; }
        
        public List<PathAction> Actions { get; set; }

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
