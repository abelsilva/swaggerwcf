using System;

namespace SwaggerWcf.Attributes
{
    public class SwaggerWcfAttribute : Attribute
    {
        /// <summary>
        /// Export this service on Swagger file
        /// </summary>
        public SwaggerWcfAttribute(string servicePath)
        {
            ServicePath = servicePath;
        }
        
        /// <summary>
        /// Path of this service
        /// </summary>
        public string ServicePath
        { get; set; }
    }
}
