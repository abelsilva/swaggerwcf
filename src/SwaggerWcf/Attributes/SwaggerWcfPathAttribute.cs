using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Attribute to describe a method exported as a path in swagger
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SwaggerWcfPathAttribute : Attribute
    {
        /// <summary>
        ///     Configures a path with more information
        /// </summary>
        /// <param name="summary">Path summary</param>
        /// <param name="description">Path description</param>
        /// <param name="operationId">Path Operation ID</param>
        /// <param name="externalDocsDescription">Path external docs description</param>
        /// <param name="externalDocsUrl">Path external docs URL</param>
        /// <param name="deprecated">Path deprecated (defaults to false)</param>
        public SwaggerWcfPathAttribute(string summary = null, string description = null, string operationId = null,
                                       string externalDocsDescription = null,
                                       string externalDocsUrl = null, bool deprecated = false,
                                       string operationPath = null)
        {
            Summary = summary;
            Description = description;
            OperationId = operationId;
            ExternalDocsDescription = externalDocsDescription;
            ExternalDocsUrl = externalDocsUrl;
            Deprecated = deprecated;
            OperationPath = operationPath;
        }

        /// <summary>
        ///     Path summary
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        ///     Path description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Path Operation ID
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        ///     Path external docs description
        /// </summary>
        public string ExternalDocsDescription { get; set; }

        /// <summary>
        ///     Path external docs URL
        /// </summary>
        public string ExternalDocsUrl { get; set; }

        /// <summary>
        ///     Path deprecated
        /// </summary>
        public bool Deprecated { get; set; }

        /// <summary>
        ///     Operation path, extends service path
        /// </summary>
        public string OperationPath { get; set; }
    }
}
