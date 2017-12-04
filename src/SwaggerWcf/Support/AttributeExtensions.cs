using SwaggerWcf.Attributes;
using SwaggerWcf.Models;

namespace SwaggerWcf.Support
{
    internal static class AttributeExtensions
    {
        public static Info AssignServiceInfoValuesTo(this SwaggerWcfServiceInfoAttribute attribute, Info info)
        {
            info.Title = attribute.Title;
            info.Version = attribute.Version;
            info.Description = attribute.Description;
            info.TermsOfService = attribute.TermsOfService;

            return info;
        }
    }
}
