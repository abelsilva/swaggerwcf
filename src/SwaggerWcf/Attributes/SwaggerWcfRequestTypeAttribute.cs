using System;
using System.Net;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Overrides the return type
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerWcfRequestTypeAttribute : Attribute
    {
        /// <summary>
        ///     Overrides the return type
        /// </summary>
        /// <param name="name">Method parameter type name</param>
        public SwaggerWcfRequestTypeAttribute(string name = null)
        {
            Name = name;
        }

        /// <summary>
        ///     Override parameter Type in Wrapped Request
        /// </summary>
        public string Name { get; set; }
    }
}
