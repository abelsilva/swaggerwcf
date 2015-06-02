using System;

namespace SwaggerWcf.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OperationAttribute : Attribute
    {
        public OperationAttribute(string summary = null, string description = null, string operationId = null,
                                  string externalDocsDescription = null,
                                  string externalDocsUrl = null)
        {
            Summary = summary;
            Description = description;
            OperationId = operationId;
            ExternalDocsDescription = externalDocsDescription;
            ExternalDocsUrl = externalDocsUrl;
        }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string OperationId { get; set; }

        public string ExternalDocsDescription { get; set; }

        public string ExternalDocsUrl { get; set; }
    }
}
