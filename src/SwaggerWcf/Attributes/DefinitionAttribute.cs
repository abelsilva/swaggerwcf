using System;

namespace SwaggerWcf.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DefinitionAttribute : Attribute
    {
        public DefinitionAttribute(string description = null,
                                   string externalDocsDescription = null,
                                   string externalDocsUrl = null)
        {
            Description = description;
            ExternalDocsDescription = externalDocsDescription;
            ExternalDocsUrl = externalDocsUrl;
        }

        public string Description { get; set; }

        public string ExternalDocsDescription { get; set; }

        public string ExternalDocsUrl { get; set; }
    }
}
