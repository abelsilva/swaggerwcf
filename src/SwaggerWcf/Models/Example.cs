using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    internal class Example
    {
        public string MimeType { get; set; }

        public string Content { get; set; }

        public void Serialize(JsonWriter writer)
        {
            if (string.IsNullOrWhiteSpace(MimeType) || string.IsNullOrWhiteSpace(Content))
                return;

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            using (JsonWriter jw = new JsonTextWriter(sw))
            {
                jw.WritePropertyName(MimeType);

                jw.WriteStartObject();

                if (!string.IsNullOrWhiteSpace(Content))
                {
                    jw.WriteRawValue(Content);
                }

                jw.WriteEndObject();

                if (jw.WriteState == WriteState.Error)
                    sb.Clear();
            }
            writer.WriteRawValue(sb.ToString());
        }
    }
}
