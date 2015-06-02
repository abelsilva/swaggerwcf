using System;

namespace SwaggerWcf.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class DeprecatedAttribute : Attribute
    {
    }
}
