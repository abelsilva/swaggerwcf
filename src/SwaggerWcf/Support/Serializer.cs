using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal class Serializer
    {
        internal static string Process(Service service)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                service.Serialize(writer);
            }
            return sb.ToString();
        }
    }
}
