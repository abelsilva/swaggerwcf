using System;
using System.Net;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Attribute to describe path responses. If none is specified, a 'default' one is created
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerWcfResponseAttribute : Attribute
    {
        /// <summary>
        ///     Configures a response for a path
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="description">Result description</param>
        /// <param name="emptyResponseOverride">Result has empty response body (override default response type)</param>
        /// <param name="headers">Optional HTTP headers returned</param>
        public SwaggerWcfResponseAttribute(string code, string description = null, bool emptyResponseOverride = false,
                                           string[] headers = null, Type responseTypeOverride = null)
        {
            Code = code;
            Description = description;
            EmptyResponseOverride = emptyResponseOverride;
            Headers = headers;
            ResponseTypeOverride = responseTypeOverride;
        }

        /// <summary>
        ///     Configures a response for a path
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="description">Result description</param>
        /// <param name="emptyResponseOverride">Result has empty response body (override default response type)</param>
        /// <param name="headers">Optional HTTP headers returned</param>
        public SwaggerWcfResponseAttribute(HttpStatusCode code, string description = null,
                                           bool emptyResponseOverride = false,
                                           string[] headers = null,
                                           Type responseTypeOverride = null)
        {
            Code = ((int) code).ToString();
            Description = description;
            EmptyResponseOverride = emptyResponseOverride;
            Headers = headers;
            ResponseTypeOverride = responseTypeOverride;
        }

        /// <summary>
        ///     Configures a response for a path
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="description">Result description</param>
        /// <param name="emptyResponseOverride">Result has empty response body (override default response type)</param>
        /// <param name="headers">Optional HTTP headers returned</param>
        public SwaggerWcfResponseAttribute(int code, string description = null, bool emptyResponseOverride = false,
                                           string[] headers = null, Type responseTypeOverride = null)
        {
            Code = code.ToString();
            Description = description;
            EmptyResponseOverride = emptyResponseOverride;
            Headers = headers;
            ResponseTypeOverride = responseTypeOverride;
        }

        /// <summary>
        ///     Result code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Result description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Result has empty response body (override default response type)
        /// </summary>
        public bool EmptyResponseOverride { get; set; }
        
        /// <summary>
        ///     Optional HTTP headers returned
        /// </summary>
        public string[] Headers { get; set; }

        /// <summary>
        ///     Override response type
        /// </summary>
        public Type ResponseTypeOverride { get; set; }
    }
}
