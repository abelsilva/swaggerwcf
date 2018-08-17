using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace SwaggerWcf.Models
{
    public class InfoLicense
    {
        public string Name { get; set; }

        public string Url
        {
            get { return _url; }
            set
            {
                Uri _ = null;
                if (Uri.TryCreate(value, UriKind.Absolute, out _))
                {
                    _url = value;
                }
                else
                {
                    throw new ArgumentException("Value must be in the format of a URL", nameof(Url));
                }
            }
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
            if (!string.IsNullOrWhiteSpace(Url))
            {
                writer.WritePropertyName("url");
                writer.WriteValue(Url);
            }
            writer.WriteEndObject();
        }
    }
}
