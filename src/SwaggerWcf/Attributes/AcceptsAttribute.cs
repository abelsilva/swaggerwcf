using System;

namespace SwaggerWcf.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class ConsumesAttribute : ContentTypeAttribute
    {
    }
}
