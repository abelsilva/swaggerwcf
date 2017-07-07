using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Describe a parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SwaggerWcfParameterAttribute : Attribute
    {
        /// <summary>
        ///     Describes a parameter
        /// </summary>
        /// <param name="required">Set parameter as required. Defaults is false.</param>
        /// <param name="description">Parameter description.</param>
        public SwaggerWcfParameterAttribute(bool required = false, string description = null, Type parameterType = null)
        {
            Required = required;
            Description = description;
            ParameterType = parameterType;
        }


        /// <summary>
        ///     Defines if this parameter is required in operations
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        ///     Description of this parameter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Override parameter type
        /// </summary>
        public Type ParameterType { get; set; }
    }
}