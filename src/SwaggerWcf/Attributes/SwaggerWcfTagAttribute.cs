using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Attribute to inject a tag into a class, method, parameter or property.
    ///     Tags can be used to categorize in Swagger UI and to hide elements using configuration
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Property,
        AllowMultiple = true)]
    public class SwaggerWcfTagAttribute : Attribute
    {
        /// <summary>
        ///     Injects a tag into an element
        /// </summary>
        /// <param name="name">Tag name</param>
        public SwaggerWcfTagAttribute(string name)
        {
            TagName = name;
        }

        /// <summary>
        ///     Tag name
        /// </summary>
        public string TagName { get; set; }
    }
}
