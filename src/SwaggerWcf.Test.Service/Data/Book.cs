using System.Runtime.Serialization;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract]
    public class Book
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public int FirstPublished { get; set; }

        [DataMember]
        public Author Author { get; set; }

        [DataMember]
        public Language Language { get; set; }
    }
}
