using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract]
    [SwaggerWcfDefinition("author")]
    public class Author
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
