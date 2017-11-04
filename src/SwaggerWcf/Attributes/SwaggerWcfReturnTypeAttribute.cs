using System;
using System.Net;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Overrides the return type
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SwaggerWcfReturnTypeAttribute : Attribute
    {
        /// <summary>
        ///     Overrides the return type
        /// </summary>
        /// <param name="returnType">Method return type</param>
        /// <param name="name">Method return type name</param>
        public SwaggerWcfReturnTypeAttribute(Type returnType = null, string name = null)
        {
            ReturnType = returnType;
            Name = name;
        }

        /// <summary>
        ///     Override Return Type
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        ///     Override Return Type in Wrapped Responses
        /// </summary>
        public string Name { get; set; }
    }
}
