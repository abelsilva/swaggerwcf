using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Describe a parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerWcfHeaderAttribute : Attribute
    {
        /// <summary>
        ///     Describes a parameter
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="required">Set parameter as required. Defaults is false.</param>
        /// <param name="description">Parameter description.</param>
        /// <param name="defaultValue">Parameter default value.</param>
        public SwaggerWcfHeaderAttribute(string name, bool required = false, string description = null, string defaultValue = null)
        {
            Name = name;
            Required = required;
            Description = description;
            DefaultValue = defaultValue;
        }

        /// <summary>
        ///     Name of this parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Defines if this parameter is required in operations
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     Description of this parameter
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        ///     Default value of this parameter
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
