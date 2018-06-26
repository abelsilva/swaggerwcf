using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;

namespace SwaggerWcf.Test.Service
{
    [ServiceContract]
    public interface IStore
    {
        #region /books
        [SwaggerWcfPath("Create book", "Create a book on the store")]
        // default Method for WebInvoke is POST
        [WebInvoke(UriTemplate = "/books", BodyStyle = WebMessageBodyStyle.Wrapped, //Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Book CreateBook([SwaggerWcfParameter(Description = "Book to be created, the id will be replaced")] Book value);

        [SwaggerWcfPath("Get books", "Retrieve all books from the store")]
        [WebGet(UriTemplate = "/books?filter={filterText}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Book[] ReadBooks(string filterText = null);

        [SwaggerWcfPath("Get book", "Retrieve a book from the store using its id")]
        [WebGet(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Book ReadBook(string id);

        [SwaggerWcfPath("Update book", "Update a book on the store")]
        [WebInvoke(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT",
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void UpdateBook(string id,
            [SwaggerWcfParameter(
                Description =
                    "Book to be updated, make sure the id in the book matches the id in the path parameter")] Book value);

        [SwaggerWcfPath("Delete book", "Delete a book on the store")]
        [WebInvoke(UriTemplate = "/books/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        [OperationContract]
        void DeleteBook(string id);

        [SwaggerWcfPath("Get book author", "Retrieve the author of a book using the book id")]
        [WebGet(UriTemplate = "/books/{id}/author", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        Author ReadBookAuthor(string id);

        #endregion

        #region /authors

        [SwaggerWcfPath("Create author", "Create an author on the store")]
        [WebInvoke(UriTemplate = "/authors", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        Author CreateAuthor(
            [SwaggerWcfParameter(Description = "Author to be created, the id will be replaced")] Author value);

        [SwaggerWcfPath("Get authors", "Retrieve all authors from the store")]
        [WebGet(UriTemplate = "/authors", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        IList<Author> ReadAuthors();

        [SwaggerWcfPath("Get author", "Retrieve an author from the store using its id")]
        [WebGet(UriTemplate = "/authors/{id}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        Author ReadAuthor(string id);

        [SwaggerWcfPath("Update author", "Update an author on the store")]
        [WebInvoke(UriTemplate = "/authors/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT",
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void UpdateAuthor(string id,
            [SwaggerWcfParameter(
                Description =
                    "Author to be updated, make sure the id in the author matches the id in the path parameter"
            )] Author value);

        [SwaggerWcfPath("Delete author", "Delete an author on the store")]
        [WebInvoke(UriTemplate = "/authors/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        [OperationContract]
        void DeleteAuthor(string id);

        [SwaggerWcfPath("Get author books", "Retrieve the author books using the author id")]
        [WebGet(UriTemplate = "/authors/{id}/books", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        Book[] ReadAuthorBooks(string id);

        #endregion

        #region /languages

        [SwaggerWcfPath("Create language", "Create an language on the store")]
        [WebInvoke(UriTemplate = "/languages", BodyStyle = WebMessageBodyStyle.Bare, Method = "POST",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        Language CreateLanguage(
            [SwaggerWcfParameter(Description = "Language to be created, the id will be replaced")] Language value);

        [SwaggerWcfPath("Get languages", "Retrieve all languages from the store")]
        [WebGet(UriTemplate = "/languages", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Language[] ReadLanguages();

        [SwaggerWcfPath("Get language", "Retrieve an language from the store using its id")]
        [WebGet(UriTemplate = "/languages/{id}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        Language ReadLanguage(string id);

        [SwaggerWcfPath("Update language", "Update an language on the store")]
        [WebInvoke(UriTemplate = "/languages/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT",
            RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        void UpdateLanguage(string id,
            [SwaggerWcfParameter(
                Description =
                    "Language to be updated, make sure the id in the language matches the id in the path parameter"
            )] Language value);

        [SwaggerWcfPath("Delete language", "Delete an language on the store")]
        [WebInvoke(UriTemplate = "/languages/{id}", BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE")]
        [OperationContract]
        void DeleteLanguage(string id);

        #endregion
    }
}