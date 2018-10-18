using System;
using System.Net;

namespace SwaggerWcf.Attributes
{
    /// <summary>
    ///     Attribute to describe path responses. If none is specified, a 'default' one is created
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerWcfResponseAttribute : Attribute
    {
        /// <summary>
        ///     Configures a response for a path
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="description">Result description</param>
        /// <param name="emptyResponseOverride">Result has empty response body (override default response type)</param>
        /// <param name="headers">Optional HTTP headers returned</param>
        /// <param name="responseTypeOverride">Optional response Type override</param>
        /// <param name="exampleMimeType">Optional Response Example MIME Type</param>
        /// <param name="exampleContent">Optional Response Example Content</param>
        public SwaggerWcfResponseAttribute(string code, string description = null, bool emptyResponseOverride = false,
            string[] headers = null, Type responseTypeOverride = null,
            string exampleMimeType = null, string exampleContent = null)
        {
            Code = code;
            Description = description;
            EmptyResponseOverride = emptyResponseOverride;
            Headers = headers;
            ResponseTypeOverride = responseTypeOverride;
            ExampleMimeType = exampleMimeType;
            ExampleContent = exampleContent;
        }

        /// <summary>
        ///     Configures a response for a path
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="description">Result description</param>
        /// <param name="emptyResponseOverride">Result has empty response body (override default response type)</param>
        /// <param name="headers">Optional HTTP headers returned</param>
        /// <param name="responseTypeOverride">Optional response Type override</param>
        /// <param name="exampleMimeType">Optional Response Example MIME Type</param>
        /// <param name="exampleContent">Optional Response Example Content</param>
        public SwaggerWcfResponseAttribute(HttpStatusCode code, string description = null,
            bool emptyResponseOverride = false,
            string[] headers = null,
            Type responseTypeOverride = null,
            string exampleMimeType = null,
            string exampleContent = null)
        {
            Code = ((int) code).ToString();
            Description = description;
            EmptyResponseOverride = emptyResponseOverride;
            Headers = headers;
            ResponseTypeOverride = responseTypeOverride;
            ExampleMimeType = exampleMimeType;
            ExampleContent = exampleContent;
        }

        /// <summary>
        ///     Configures a response for a path
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="description">Result description</param>
        /// <param name="emptyResponseOverride">Result has empty response body (override default response type)</param>
        /// <param name="headers">Optional HTTP headers returned</param>
        /// <param name="responseTypeOverride">Optional response Type override</param>
        /// <param name="exampleMimeType">Optional Response Example MIME Type</param>
        /// <param name="exampleContent">Optional Response Example Content</param>
        public SwaggerWcfResponseAttribute(int code, string description = null, bool emptyResponseOverride = false,
            string[] headers = null, Type responseTypeOverride = null,
            string exampleMimeType = null, string exampleContent = null)
        {
            Code = code.ToString();
            Description = description;
            EmptyResponseOverride = emptyResponseOverride;
            Headers = headers;
            ResponseTypeOverride = responseTypeOverride;
            ExampleMimeType = exampleMimeType;
            ExampleContent = exampleContent;
        }

        /// <summary>
        ///     Result code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        ///     Result description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Result has empty response body (override default response type)
        /// </summary>
        public bool EmptyResponseOverride { get; set; }

        /// <summary>
        ///     Optional HTTP headers returned
        /// </summary>
        public string[] Headers { get; set; }

        /// <summary>
        ///     Example MIME Type
        /// </summary>
        public string ExampleMimeType { get; set; }

        /// <summary>
        ///     Example Content
        /// </summary>
        public string ExampleContent { get; set; }

        /// <summary>
        ///     Override response type
        /// </summary>
        public Type ResponseTypeOverride { get; set; }

        public override bool Equals(object obj)
        {
            SwaggerWcfResponseAttribute other = obj as SwaggerWcfResponseAttribute;
            if (other != null)
            {
                return string.Equals(Code, other.Code) &&
                       string.Equals(Description, other.Description) &&
                       EmptyResponseOverride == other.EmptyResponseOverride &&
                       Equals(Headers, other.Headers) &&
                       ResponseTypeOverride == other.ResponseTypeOverride &&
                       ExampleMimeType == other.ExampleMimeType &&
                       ExampleContent == other.ExampleContent;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ EmptyResponseOverride.GetHashCode();
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ResponseTypeOverride != null ? ResponseTypeOverride.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ExampleMimeType?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ ExampleContent?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }
}