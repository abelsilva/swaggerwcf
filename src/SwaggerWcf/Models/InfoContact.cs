using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace SwaggerWcf.Models
{
    public class InfoContact
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Url
        {
            get => _url;
            set => _url = Uri.TryCreate(value, UriKind.Absolute, out Uri _)
                    ? value
                    : throw new ArgumentException("Value must be in the format of a URL", nameof(Url));
        }

        private string _url;

        internal void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                writer.WritePropertyName("name");
                writer.WriteValue(Name);
            }
            if (!string.IsNullOrWhiteSpace(Email))
            {
                writer.WritePropertyName("email");
                writer.WriteValue(Email);
            }
            if (!string.IsNullOrWhiteSpace(Url))
            {
                writer.WritePropertyName("url");
                writer.WriteValue(Url);
            }
            writer.WriteEndObject();
        }
    }
}
