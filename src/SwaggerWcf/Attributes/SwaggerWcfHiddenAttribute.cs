using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Attribute to hide a method, class, parameter or property from Swagger.
    ///     This overrides any tag-based settings.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SwaggerWcfHiddenAttribute : Attribute
    {
    }
}
