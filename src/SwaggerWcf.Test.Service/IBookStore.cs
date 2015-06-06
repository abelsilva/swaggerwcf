using System.ServiceModel;
using System.ServiceModel.Web;
using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;

namespace SwaggerWcf.Test.Service
{
    [ServiceContract]
    public interface IBookStore
    {
        [SwaggerWcfPath("Create book", "Create a book on the store")]
        [WebInvoke(UriTemplate = "/books", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Book CreateBook(Book value);

        [SwaggerWcfPath("Get books", "Retrieve all books from the store")]
        [WebGet(UriTemplate = "/books", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Book[] ReadBooks();

        [SwaggerWcfPath("Get book", "Retrieve a book from the store using its id")]
        [WebGet(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Book ReadBook(string id);

        [SwaggerWcfPath("Update book", "Update a book on the store")]
        [WebInvoke(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT",
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void UpdateBook(string id, Book value);

        [SwaggerWcfPath("Delete book", "Delete a book on the store")]
        [WebInvoke(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        [OperationContract]
        void DeleteBook(string id);
    }
}
