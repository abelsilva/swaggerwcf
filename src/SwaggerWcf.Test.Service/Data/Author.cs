using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract]
    [SwaggerWcfDefinition("author")]
    public class Author : BaseEntity
    {
        [DataMember]
        public string Name { get; set; }
    }
}
