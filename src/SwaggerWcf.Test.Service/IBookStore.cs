using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;

namespace SwaggerWcf.Test.Service
{
    [ServiceContract]
    public interface IBookStore
    {
        [WebInvoke(UriTemplate = "/books", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Operation("Create book", "Create a book on the store")]
        [OperationContract]
        Book CreateBook(Book value);

        [WebGet(UriTemplate = "/books", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Operation("Get books", "Retrieve all books from the store")]
        [OperationContract]
        Book[] ReadBooks();
        
        [WebGet(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Operation("Get book", "Retrieve a book from the store using its id")]
        [OperationContract]
        Book ReadBook(string id);
        
        [WebInvoke(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT", RequestFormat = WebMessageFormat.Json)]
        [Operation("Update book", "Update a book on the store")]
        [OperationContract]
        void UpdateBook(string id, Book value);

        [WebInvoke(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        [Operation("Delete book", "Delete a book on the store")]
        [OperationContract]
        void DeleteBook(string id);
    }
}
