using System;

namespace SwaggerWcf.Attributes
{
    public abstract class ContentTypeAttribute : Attribute
    {
        public string ContentType { get; set; }
    }
}
