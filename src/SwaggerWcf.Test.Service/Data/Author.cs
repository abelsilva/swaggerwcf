using System.Runtime.Serialization;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract]
    public class Author
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
