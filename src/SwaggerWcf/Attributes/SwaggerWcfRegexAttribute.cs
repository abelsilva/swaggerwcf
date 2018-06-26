using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Apply a Regular Expression
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SwaggerWcfRegexAttribute : Attribute
    {
        /// <summary>
        ///     Applies a Regular Expression to a field or property
        /// </summary>
        /// <param name="regex">Regular Expression</param>
        public SwaggerWcfRegexAttribute(string regex)
        {
            Regex = regex;
        }

        /// <summary>
        ///     Regular Expression
        /// </summary>
        public string Regex { get; set; }
    }
}
