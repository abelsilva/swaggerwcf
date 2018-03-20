using SwaggerWcf.Models;
using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    /// Provides metadata about the API. The metadata can be used by the clients if needed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SwaggerWcfServiceInfoAttribute : Attribute
    {
        /// <summary>
        /// Required. The title of the application.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Required Provides the version of the application API (not to be confused with the specification version).
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// A short description of the application. GFM syntax can be used for rich text representation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Terms of Service for the API.
        /// </summary>
        public string TermsOfService { get; set; }

        /// <summary>
        /// Assigns service info values
        /// </summary>
        /// <param name="title">The title of the application</param>
        /// <param name="version">Provides the version of the application API (not to be confused with the specification version)</param>
        public SwaggerWcfServiceInfoAttribute(string title, string version)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));

            if (string.IsNullOrWhiteSpace(version))
                throw new ArgumentNullException(nameof(version));

            Title = title;
            Version = version;
        }

        public static explicit operator Info(SwaggerWcfServiceInfoAttribute attribute) =>
            new Info
            {
                Title = attribute.Title,
                Version = attribute.Version,
                Description = attribute.Description,
                TermsOfService = attribute.TermsOfService
            };
    }
}