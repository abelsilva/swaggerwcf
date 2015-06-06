using System;

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
        /// <param name="headers">Optional HTTP headers returned</param>
        public SwaggerWcfResponseAttribute(string code, string description = null, string[] headers = null)
        {
            Code = code;
            Description = description;
            Headers = headers;
        }

        /// <summary>
        ///     Configures a response for a path
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="description">Result description</param>
        /// <param name="headers">Optional HTTP headers returned</param>
        public SwaggerWcfResponseAttribute(int code, string description = null, string[] headers = null)
        {
            Code = code.ToString();
            Description = description;
            Headers = headers;
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
        ///     Optional HTTP headers returned
        /// </summary>
        public string[] Headers { get; set; }
    }
}
