using System;
using System.Linq;
using System.Reflection;

namespace SwaggerWcf.Support
{
    public static class MethodInfoExtensions
    {
        public static string GetWrappedName(this MethodInfo methodInfo)
        {
            return methodInfo.Name.Contains('.')
                ? methodInfo.Name.Substring(methodInfo.Name.LastIndexOf(".", StringComparison.Ordinal) + 1)
                : methodInfo.Name;
        }
    }
}