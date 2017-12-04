using SwaggerWcf.Attributes;
using SwaggerWcf.Models;
using SwaggerWcf.Support;
using System;
using System.Reflection;

namespace SwaggerWcf.AutoGenerate.Test
{
    public static class Extensions
    {
        public static Info GetServiceInfo(this Type type)
        {
            var serviceInfoAttr = type.GetCustomAttribute<SwaggerWcfServiceInfoAttribute>();

            Info info = serviceInfoAttr.AssignServiceInfoValuesTo(new Info());

            var contactInfoAttr = type.GetCustomAttribute<SwaggerWcfContactInfoAttribute>();
            if (contactInfoAttr != null)
            {
                info.Contact = (InfoContact)contactInfoAttr;
            }

            var licenseInfoAttr = type.GetCustomAttribute<SwaggerWcfLicenseInfoAttribute>();
            if (licenseInfoAttr != null)
            {
                info.License = (InfoLicense)licenseInfoAttr;
            }

            return info;
        }

        public static string GetBasePath(this Type type) =>
            type.GetCustomAttribute<SwaggerWcfAttribute>().BasePath;
    }
}
