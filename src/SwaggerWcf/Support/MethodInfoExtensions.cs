using System;
using System.Linq;
using System.Reflection;
using SwaggerWcf.Attributes;

namespace SwaggerWcf.Support
{
    public static class MethodInfoExtensions
    {
        public static string GetWrappedName(this MethodInfo implementation, MethodInfo declaration)
        {
            return implementation.GetCustomAttribute<SwaggerWcfRequestTypeAttribute>()?.Name
                   ?? declaration.GetCustomAttribute<SwaggerWcfRequestTypeAttribute>()?.Name
                   ?? (implementation.Name.Contains('.')
                       ? implementation.Name.Substring(implementation.Name.LastIndexOf(".", StringComparison.Ordinal) + 1)
                       : implementation.Name);
        }
    }
}