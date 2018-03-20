using SwaggerWcf.Models;
using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    /// Provides values for License information for the exposed API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SwaggerWcfLicenseInfoAttribute : Attribute
    {
        /// <summary>
        /// Required. The license name used for the API.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A URL to the license used for the API. MUST be in the format of a URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///  Provides values for License information for the exposed API.
        /// </summary>
        /// <param name="name">The license name used for the API.</param>
        public SwaggerWcfLicenseInfoAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public static explicit operator InfoLicense(SwaggerWcfLicenseInfoAttribute attr)
        {
            return new InfoLicense
            {
                Name = attr.Name,
                Url = attr.Url
            };
        }
    }
}