using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class PathAction
    {
        public PathAction()
        {
            Tags = new List<string>();
            Consumes = new List<string>();
            Produces = new List<string>();
            Parameters = new List<ParameterBase>();
            Responses = new List<Response>();
            Schemes = new List<string>();
        }

        public string Id { get; set; }
        
        public List<string> Tags { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public ExternalDocumentation ExternalDocs { get; set; }

        public string OperationId { get; set; }

        public List<string> Consumes { get; set; }

        public List<string> Produces { get; set; }

        public List<ParameterBase> Parameters { get; set; }

        public List<Response> Responses { get; set; }

        public List<string> Schemes { get; set; }

        public bool Deprecated { get; set; }
        
        public List<KeyValuePair<string, string[]>> Security { get; set; }

        public void Serialize(JsonWriter writer)
        {
            writer.WritePropertyName(Id);

            writer.WriteStartObject();
            
            if (Tags != null && Tags.Any())
            {
                writer.WritePropertyName("tags");
                writer.WriteStartArray();
                foreach (string tag in Tags)
                {
                    writer.WriteValue(tag);
                }
                writer.WriteEndArray();
            }

            if (!string.IsNullOrWhiteSpace(Summary))
            {
                writer.WritePropertyName("summary");
                writer.WriteValue(Summary);
            }

            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }

            if (ExternalDocs != null)
            {
                writer.WritePropertyName("externalDocs");
                ExternalDocs.Serialize(writer);
            }

            if (!string.IsNullOrWhiteSpace(OperationId))
            {
                writer.WritePropertyName("operationId");
                writer.WriteValue(OperationId);
            }

            if (Consumes != null && Consumes.Any())
            {
                writer.WritePropertyName("consumes");
                writer.WriteStartArray();
                foreach (string cons in Consumes)
                {
                    writer.WriteValue(cons);
                }
                writer.WriteEndArray();
            }

            if (Produces != null && Produces.Any())
            {
                writer.WritePropertyName("produces");
                writer.WriteStartArray();
                foreach (string prod in Produces)
                {
                    writer.WriteValue(prod);
                }
                writer.WriteEndArray();
            }

            if (Parameters != null && Parameters.Any())
            {
                writer.WritePropertyName("parameters");
                writer.WriteStartArray();
                foreach (ParameterBase p in Parameters)
                {
                    p.Serialize(writer);
                }
                writer.WriteEndArray();
            }

            if (Responses != null && Responses.Any())
            {
                writer.WritePropertyName("responses");
                writer.WriteStartObject();
                foreach (Response r in Responses)
                {
                    r.Serialize(writer);
                }
                writer.WriteEndObject();
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

            if (Deprecated)
            {
                writer.WritePropertyName("deprecated");
                writer.WriteValue(Deprecated);
            }

            if (Security != null && Security.Any())
            {
                writer.WritePropertyName("security");
                writer.WriteStartArray();

                foreach (var security in Security)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(security.Key);
                    writer.WriteStartArray();

                    if (security.Value != null && security.Value.Any())
                    {
                        foreach (var scopename in security.Value)
                        {
                            writer.WriteValue(scopename);
                        }
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }
    }
}
