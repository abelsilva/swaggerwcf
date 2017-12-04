using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    /// Attribute to enable a class or interface to be scanned by SwaggerWcf
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class SwaggerWcfAttribute : Attribute
    {
        /// <summary>
        /// The base path on which the API is served, which is relative to the host. If it is not included,
        /// the API is served directly under the host. The value MUST start with a leading slash (/). The
        /// basePath does not support path templating.
        /// </summary>
        public string BasePath { get; }

        /// <summary>
        /// Export this service on Swagger file with service base path of "/"
        /// </summary>
        public SwaggerWcfAttribute()
            : this("/")
        { }

        /// <summary>
        /// Export this service on Swagger file with service base path
        /// </summary>
        /// <param name="basePath">Service path</param>
        public SwaggerWcfAttribute(string basePath)
        {
            BasePath = basePath.StartsWith("/") ? basePath : $"/{basePath}";
        }
    }
}
