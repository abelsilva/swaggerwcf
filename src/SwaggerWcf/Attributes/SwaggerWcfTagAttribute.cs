using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Attribute to inject a tag into a class, method, parameter or property.
    ///     Tags can be used to categorize in Swagger UI and to hide elements using configuration
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter,
        AllowMultiple = true)]
    public class SwaggerWcfTagAttribute : Attribute
    {
        /// <summary>
        ///     Injects a tag into an element
        /// </summary>
        /// <param name="name">Tag name</param>
        /// <param name="hideFromSpec">Hide tag from spec</param>
        public SwaggerWcfTagAttribute(string name, bool hideFromSpec = false)
        {
            TagName = name;
            HideFromSpec = hideFromSpec;
        }

        /// <summary>
        ///     Tag name
        /// </summary>
        public string TagName { get; set; }
        
        /// <summary>
        ///     Hide tag from spec
        /// </summary>
        public bool HideFromSpec { get; set; }
    }
}
