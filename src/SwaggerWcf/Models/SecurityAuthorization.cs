using System.Collections.Generic;
using Newtonsoft.Json;

namespace SwaggerWcf.Models
{
    public class SecurityAuthorization
    {
        /// <summary>
        /// (Required) The type of the security scheme. Valid values are "basic", "apiKey" or "oauth2".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A short description for security scheme.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// (Required) The name of the header or query parameter to be used.
        /// WARNING: Use only, when <see cref="Type"/> equals "apiKey"
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// (Required) The location of the API key. Valid values are "query" or "header".
        /// WARNING: Use only, when <see cref="Type"/> equals "apiKey"
        /// </summary>
        public string In { get; set; }

        /// <summary>
        /// (Required) The flow used by the OAuth2 security scheme. Valid values are "implicit", "password", "application" or "accessCode".
        /// WARNING: Use only, when <see cref="Type"/> equals "oauth2"
        /// </summary>
        public string Flow { get; set; }

        /// <summary>
        /// (Required) The authorization URL to be used for this flow. This SHOULD be in the form of a URL.
        /// WARNING: Use only, when <see cref="Type"/> equals "oauth2" and <see cref="Flow"/> equals "implicit" or "accessCode"
        /// </summary>
        public string AuthorizationUrl { get; set; }

        /// <summary>
        /// (Required) The token URL to be used for this flow. This SHOULD be in the form of a URL.
        /// WARNING: Use only, when <see cref="Type"/> equals "oauth2" and <see cref="Flow"/> equals "password" or "application" or "accessCode"
        /// </summary>
        public string TokenUrl { get; set; }

        /// <summary>
        /// (Required) The available scopes for the OAuth2 security scheme.
        /// This maps between a name of a scope to a short description of it (as the value of the property).
        /// WARNING: Use only, when <see cref="Type"/> equals "oauth2"
        /// </summary>
        public Dictionary<string, string> Scopes { get; set; }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();

            if (Type != null)
            {
                writer.WritePropertyName("type");
                writer.WriteValue(Type);
            }

            if (Description != null)
            {
                writer.WritePropertyName("description");
                writer.WriteValue(Description);
            }

            if (Name != null)
            {
                writer.WritePropertyName("name");
                writer.WriteValue(Name);
            }

            if (In != null)
            {
                writer.WritePropertyName("in");
                writer.WriteValue(In);
            }

            if (Flow != null)
            {
                writer.WritePropertyName("flow");
                writer.WriteValue(Flow);
            }

            if (AuthorizationUrl != null)
            {
                writer.WritePropertyName("authorizationUrl");
                writer.WriteValue(AuthorizationUrl);
            }

            if (TokenUrl != null)
            {
                writer.WritePropertyName("tokenUrl");
                writer.WriteValue(TokenUrl);
            }

            if (Scopes != null)
            {
                writer.WritePropertyName("scopes");
                writer.WriteStartObject();

                foreach (var scope in Scopes)
                {
                    writer.WritePropertyName(scope.Key);
                    writer.WriteValue(scope.Value);
                }

                writer.WriteEndObject();
            }
         
            writer.WriteEndObject();
        }
    }
}
