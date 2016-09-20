using System;
using System.Net;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Overrides the return type
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerWcfContentTypesAttribute : Attribute
    {
        /// <summary>
        ///     Overrides the return type
        /// </summary>
        /// <param name="returnType">Method return type</param>
        public SwaggerWcfContentTypesAttribute(string[] consumeTypes = null, string[] produceTypes = null)
        {
            ConsumeTypes = consumeTypes;
            ProduceTypes = produceTypes;
        }

        /// <summary>
        ///     Override Response Content-Types
        /// </summary>
        public string[] ConsumeTypes { get; set; }

        /// <summary>
        ///     Override Response Content-Types
        /// </summary>
        public string[] ProduceTypes { get; set; }
    }
}
