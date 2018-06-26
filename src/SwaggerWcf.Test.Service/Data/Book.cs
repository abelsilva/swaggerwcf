using System.ComponentModel;
using System.Runtime.Serialization;
using SwaggerWcf.Attributes;

namespace SwaggerWcf.Test.Service.Data
{
    [DataContract]
    [Description("Book with title, first publish date, author and language")]
    [SwaggerWcfDefinition(ExternalDocsUrl = "http://en.wikipedia.org/wiki/Book", ExternalDocsDescription = "Description of a book")]
    public class Book
    {
        [DataMember]
        [Description("Book ID")]
        [SwaggerWcfRegex("[\\w\\d]*")]
        public string Id { get; set; }

        [DataMember]
        [Description("Book Title")]
        public string Title { get; set; }

        [DataMember]
        [Description("Book First Publish Date")]
        public int FirstPublished { get; set; }

        [DataMember]
        [Description("Book Author")]
        public Author Author { get; set; }

        [DataMember]
        [Description("Book Language")]
        public Language Language { get; set; }

        [DataMember]
        [Description("Book Tags")]
        public string[] Tags { get; set; }
    }
}
