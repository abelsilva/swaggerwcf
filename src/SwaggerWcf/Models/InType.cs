using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SwaggerWcf.Models
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum InType
    {
        Unknown = 0,
        Body = 1,
        Path = 2,
        Query = 3,
        FormData = 4,
        Header = 5
    }
}