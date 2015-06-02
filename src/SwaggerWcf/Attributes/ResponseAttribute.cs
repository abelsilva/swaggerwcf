using System;

namespace SwaggerWcf.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ResponseAttribute : Attribute
    {
        public ResponseAttribute(string code, string reason = null, string[] headers = null)
        {
            Code = code;
            Description = reason;
        }
        public ResponseAttribute(int code, string reason = null, string[] headers = null)
        {
            Code = code.ToString();
            Description = reason;
            Headers = headers;
        }

        public string Code { get; set; }

        public string Description { get; set; }

        public string[] Headers { get; set; }
    }
}
