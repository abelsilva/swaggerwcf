using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SwaggerWcf.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public abstract class ParameterBase
    {
        public string Name { get; set; }

        // body, path, query, formData (query and formData can have CollectionFormat as multi)
        public InType In { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public abstract void Serialize(JsonWriter writer);
        public abstract void Serialize(JsonWriter writer, bool skipStartEndObject);
    }
}
