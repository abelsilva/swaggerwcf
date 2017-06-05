using SwaggerWcf.Models;
using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Describe a query
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerWcfQueryAttribute : Attribute
    {
        /// <summary>
        ///     Describes a query
        /// </summary>
        /// <param name="name">Query name</param>
        /// <param name="required">Set query as required. Defaults is false.</param>
        /// <param name="description">Query description.</param>
        /// <param name="defaultValue">Query default value.</param>
        public SwaggerWcfQueryAttribute(string name, bool required = false, string description = null, string defaultValue = null, ParameterType type = ParameterType.Unknown)
        {
            Name = name;
            Required = required;
            Description = description;
            DefaultValue = defaultValue;
            Type = type;
        }

        /// <summary>
        ///     Name of this query
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Defines if this query is required in operations
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     Description of this query
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Default value of this query
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        ///     Type of this query
        /// </summary>
        public ParameterType Type { get; set; }
    }
}
