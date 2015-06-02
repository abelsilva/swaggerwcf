using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    public class Info
    {
        public string Version { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string TermsOfService { get; set; }

        public InfoContact Contact { get; set; }

        public InfoLicense License { get; set; }

        internal void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Version))
            {
                writer.WritePropertyName("version");
                writer.WriteValue(Version);
            }
            if (!string.IsNullOrWhiteSpace(Title))
            {
                writer.WritePropertyName("title");
                writer.WriteValue(Title);
            }
            if (!string.IsNullOrWhiteSpace(Description))
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }
            if (!string.IsNullOrWhiteSpace(TermsOfService))
            {
                writer.WritePropertyName("termsOfService");
                writer.WriteValue(TermsOfService);
            }

            if (Contact != null)
            {
                writer.WritePropertyName("contact");
                Contact.Serialize(writer);
            }

            if (License != null)
            {
                writer.WritePropertyName("license");
                License.Serialize(writer);
            }

            writer.WriteEndObject();
        }
    }
}
