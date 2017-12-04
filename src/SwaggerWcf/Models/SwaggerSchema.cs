using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwaggerWcf.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public sealed class SwaggerSchema
    {
        [JsonProperty(Required = Required.Always)]
        public string Swagger => "2.0";

        [JsonProperty(Required = Required.Always)]
        public Info Info { get; set; }

        /// <summary>
        /// The host (name or ip) serving the API. This MUST be the host only and does not include the scheme
        /// nor sub-paths. It MAY include a port. If the host is not included, the host serving the documentation
        /// is to be used (including the port). The host does not support path templating.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The base path on which the API is served, which is relative to the host. If it is not included,
        /// the API is served directly under the host. The value MUST start with a leading slash (/). The
        /// basePath does not support path templating.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// The transfer protocol of the API. Values MUST be from the list: "http", "https", "ws", "wss". If
        /// the schemes is not included, the default scheme to be used is the one used to access the Swagger
        /// definition itself.
        /// </summary>
        public IEnumerable<Scheme> Schemes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can consume. This is global to all APIs but can be overridden on
        /// specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        public string[] Consumes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can produce. This is global to all APIs but can be overridden on
        /// specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        public string[] Produces { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<string, PathItem> Paths { get; set; }

        public IEnumerable<Definition> Definitions { get; set; }

        public SecurityDefinitions SecurityDefinitions { get; set; }

        public void Serialize(JsonWriter writer)
        {
            /*
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
            */
        }

        //private void WritePaths(JsonWriter writer)
        //{
        //    writer.WriteStartObject();
        //    foreach (Path p in Paths.OrderBy(p => p.Id))
        //    {
        //        p.Serialize(writer);
        //    }
        //    writer.WriteEndObject();
        //}

        //private void WriteDefinitions(JsonWriter writer)
        //{
        //    writer.WriteStartObject();
        //    foreach (Definition d in Definitions.OrderBy(d => d.Schema.Name))
        //    {
        //        d.Serialize(writer);
        //    }
        //    writer.WriteEndObject();
        //}

        //private void WriteSecurityDefinitions(JsonWriter writer)
        //{
        //    writer.WriteStartObject();
        //    foreach (var d in SecurityDefinitions)
        //    {
        //        writer.WritePropertyName(d.Key);
        //        d.Value.Serialize(writer);
        //    }
        //    writer.WriteEndObject();
        //}
    }
}
