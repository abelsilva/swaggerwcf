using System.Runtime.Serialization;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract]
    public class Language
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
