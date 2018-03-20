using SwaggerWcf.Attributes;
using SwaggerWcf.Models;
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

        internal static Info GetServiceInfo(this TypeInfo typeInfo)
        {
            var infoAttr = typeInfo.GetCustomAttribute<SwaggerWcfServiceInfoAttribute>() ??
                throw new ArgumentException($"{typeInfo.FullName} does not have {nameof(SwaggerWcfServiceInfoAttribute)}");

            var info = (Info)infoAttr;

            var contactAttr = typeInfo.GetCustomAttribute<SwaggerWcfContactInfoAttribute>();
            if (contactAttr != null)
            {
                info.Contact = (InfoContact)contactAttr;
            }

            var licenseAttr = typeInfo.GetCustomAttribute<SwaggerWcfLicenseInfoAttribute>();
            if (licenseAttr != null)
            {
                info.License = (InfoLicense)licenseAttr;
            }

            return info;
        }
    }
}
