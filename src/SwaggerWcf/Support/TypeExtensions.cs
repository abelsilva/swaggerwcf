using System;
using System.Linq;

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
    }
}
