using SwaggerWcf.Models;
using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    /// Provides values for Contact information for the exposed API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SwaggerWcfContactInfoAttribute : Attribute
    {
        /// <summary>
        /// The identifying name of the contact person/organization.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email address of the contact person/organization. MUST be in the format of an email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The URL pointing to the contact information. MUST be in the format of a URL.
        /// </summary>
        public string Url { get; set; }

        public static explicit operator InfoContact(SwaggerWcfContactInfoAttribute attr)
        {
            return new InfoContact
            {
                Name = attr.Name,
                Url = attr.Url,
                Email = attr.Email
            };
        }
    }
}