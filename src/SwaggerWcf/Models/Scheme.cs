using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SwaggerWcf.Models
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum Scheme
    {
        Http,
        Https,
        Ws,
        Wss
    }
}
