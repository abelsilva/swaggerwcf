using System;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    /// Attribute to link operation to a security definition
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerWcfSecurityAttribute : Attribute
    {
        /// <summary>
        /// Specify security definition for this operation
        /// </summary>
        /// <param name="securityDefinitionName">Name of the Security Definition</param>
        /// <param name="scopes">Scopes of the security definition</param>
        public SwaggerWcfSecurityAttribute(string securityDefinitionName, params string[] scopes)
        {
            SecurityDefinitionName = securityDefinitionName;
            SecurityDefinitionScopes = scopes;
        }

        /// <summary>
        /// Name of the Security Definition
        /// </summary>
        public string SecurityDefinitionName { get; set; }

        /// <summary>
        /// Scopes of the Security Definition
        /// </summary>
        public string[] SecurityDefinitionScopes { get; set; }

    }
}