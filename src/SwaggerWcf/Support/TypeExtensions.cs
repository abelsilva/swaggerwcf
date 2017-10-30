using SwaggerWcf.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace SwaggerWcf.Support
{
    internal static class TypeExtensions
    {
        public static Type GetEnumerableType(this Type type)
        {
            Type elementType = type.GetElementType();
            if (elementType != null)
                return elementType;

            Type[] genericArguments = type.GetGenericArguments();

            return genericArguments.Any() ? genericArguments[0] : null;
        }

        public static string GetModelName(this Type type) =>
            type.GetCustomAttribute<SwaggerWcfDefinitionAttribute>()?.ModelName ?? type.FullName;

        public static string GetModelWrappedName(this Type type) =>
            type.GetCustomAttribute<SwaggerWcfDefinitionAttribute>()?.ModelName ?? type.FullName;
    }
}
