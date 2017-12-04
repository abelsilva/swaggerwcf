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
        /// Path of this service
        /// </summary>
        public string ServicePath { get; set; }

        /// <summary>
        /// Export this service on Swagger file with service base path of "/"
        /// </summary>
        public SwaggerWcfAttribute()
            : this("/")
        { }

        /// <summary>
        /// Export this service on Swagger file with service base path
        /// </summary>
        /// <param name="servicePath">Service path</param>
        public SwaggerWcfAttribute(string servicePath)
        {
            ServicePath = servicePath.StartsWith("/") ? servicePath : $"/{servicePath}";
        }
    }
}
