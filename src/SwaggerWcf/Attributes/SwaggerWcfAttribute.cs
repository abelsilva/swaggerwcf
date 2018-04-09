using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    /// Attribute to enable a class or interface to be scanned by SwaggerWcf
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SwaggerWcfAttribute : Attribute
    {
        /// <summary>
        /// The base path on which the API is served, which is relative to the host. If it is not included,
        /// the API is served directly under the host. The value MUST start with a leading slash (/). The
        /// basePath does not support path templating.
        /// </summary>
        public string ServicePath { get; set; }

        /// <summary>
        /// Export this service on Swagger file
        /// </summary>
        public SwaggerWcfAttribute()
        {
        }

        /// <summary>
        /// Export this service on Swagger file with base path of <paramref name="servicePath"/>
        /// </summary>
        /// <param name="servicePath">Service path</param>
        public SwaggerWcfAttribute(string servicePath)
        {
            ServicePath = servicePath.StartsWith("/") ? servicePath : $"/{servicePath}";
        }
    }
}
