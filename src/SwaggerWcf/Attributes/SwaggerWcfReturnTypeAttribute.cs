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
        public SwaggerWcfReturnTypeAttribute(Type returnType = null)
        {
            ReturnType = returnType;
        }

        /// <summary>
        ///     Override Return Type
        /// </summary>
        public Type ReturnType { get; set; }
    }
}
