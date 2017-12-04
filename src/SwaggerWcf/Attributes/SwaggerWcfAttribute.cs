using SwaggerWcf.Models;
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
        public string BasePath { get; set; }

        /// <summary>
        /// The host (name or ip) serving the API. This MUST be the host only and does not include the scheme
        /// nor sub-paths. It MAY include a port. If the host is not included, the host serving the documentation
        /// is to be used (including the port). The host does not support path templating.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The transfer protocol of the API. Values MUST be from the list: "http", "https", "ws", "wss". If
        /// the schemes is not included, the default scheme to be used is the one used to access the Swagger
        /// definition itself.
        /// </summary>
        public Scheme[] Schemes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can consume. This is global to all APIs but can be overridden on
        /// specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        public string[] Consumes { get; set; }

        /// <summary>
        /// A list of MIME types the APIs can produce. This is global to all APIs but can be overridden on
        /// specific API calls. Value MUST be as described under Mime Types.
        /// </summary>
        public string[] Produces { get; set; }
    }
}
