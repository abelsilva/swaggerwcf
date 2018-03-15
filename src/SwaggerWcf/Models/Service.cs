using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal sealed class Service
    {
        public Service()
        {
            Swagger = "2.0";
            Paths = new List<Path>();
            Definitions = new List<Definition>();
        }

        public string Swagger { get; set; }

        public Info Info { get; set; }

        public string Host { get; set; }

        public string BasePath { get; set; }

        public List<string> Schemes { get; set; }

        public List<Path> Paths { get; set; }

        public List<Definition> Definitions { get; set; }

        public SecurityDefinitions SecurityDefinitions { get; set; }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("swagger");
            writer.WriteValue(Swagger);
            if (Info != null)
            {
                writer.WritePropertyName("info");
                Info.Serialize(writer);
            }
            if (Host != null)
            {
                writer.WritePropertyName("host");
                writer.WriteValue(Host);
            }
            if (BasePath != null)
            {
                writer.WritePropertyName("basePath");
                writer.WriteValue(BasePath);
            }
            if (Schemes != null && Schemes.Any())
            {
                writer.WritePropertyName("schemes");
                writer.WriteStartArray();
                foreach (string sch in Schemes)
                {
                    writer.WriteValue(sch);
                }
                writer.WriteEndArray();
            }
            if (Paths != null && Paths.Any())
            {
                writer.WritePropertyName("paths");
                WritePaths(writer);
            }

            if (Definitions != null && Definitions.Any())
            {
                writer.WritePropertyName("definitions");
                WriteDefinitions(writer);
            }

            if (SecurityDefinitions != null && SecurityDefinitions.Any())
            {
                writer.WritePropertyName("securityDefinitions");
                WriteSecurityDefinitions(writer);
            }

            writer.WriteEndObject();
        }

        private void WritePaths(JsonWriter writer)
        {
            writer.WriteStartObject();
            foreach (Path p in Paths.OrderBy(p => p.Id))
            {
                p.Serialize(writer);
            }
            writer.WriteEndObject();
        }

        private void WriteDefinitions(JsonWriter writer)
        {
            writer.WriteStartObject();
            foreach (Definition d in Definitions.OrderBy(d => d.Schema.Name))
            {
                d.Serialize(writer);
            }
            writer.WriteEndObject();
        }

        private void WriteSecurityDefinitions(JsonWriter writer)
        {
            writer.WriteStartObject();
            foreach (var d in SecurityDefinitions)
            {
                writer.WritePropertyName(d.Key);
                d.Value.Serialize(writer);
            }
            writer.WriteEndObject();
        }
    }
}
