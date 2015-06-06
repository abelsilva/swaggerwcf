using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;

namespace SwaggerWcf.Test.Service
{
    [SwaggerWcf("/v1/rest")]
    public class BookStore : IBookStore
    {
        [SwaggerWcfTag("Books")]
        [SwaggerWcfResponse(201, "Book created, value in the response body with id updated")]
        [SwaggerWcfResponse(500, "Internal error (can be forced using ERROR_500 as book title)")]
        public Book CreateBook(Book value)
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return null;

            if (!string.IsNullOrWhiteSpace(value.Title) && value.Title == "ERROR_500")
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }

            value.Id = Guid.NewGuid().ToString("N");
            Store.Books.Add(value);

            woc.OutgoingResponse.StatusCode = HttpStatusCode.Created;

            return value;
        }

        [SwaggerWcfTag("Books")]
        [SwaggerWcfResponse(200, "Book found, value in the response body")]
        [SwaggerWcfResponse(404, "Book not found")]
        [SwaggerWcfResponse(500, "Internal error (can be forced using ERROR_500 as book id)")]
        public Book ReadBook(string id)
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return null;

            Book book = Store.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                return book;
            }

            if (id == "ERROR_500")
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return null;
            }

            woc.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            return null;
        }

        [SwaggerWcfTag("Books")]
        [SwaggerWcfResponse(200, "Book found, value in the response body")]
        [SwaggerWcfResponse(204, "No books")]
        public Book[] ReadBooks()
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return null;

            if (Store.Books.Any())
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                return Store.Books.ToArray();
            }

            woc.OutgoingResponse.StatusCode = HttpStatusCode.NoContent;
            return null;
        }

        [SwaggerWcfTag("Books")]
        [SwaggerWcfResponse(204, "Book updated")]
        [SwaggerWcfResponse(404, "Book not found")]
        [SwaggerWcfResponse(500, "Internal error (can be forced using ERROR_500 as book id)")]
        public void UpdateBook(string id, Book value)
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return;

            Book book = Store.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.NoContent;
                Store.Books.Remove(book);
                Store.Books.Add(value);
            }
            else if (id == "ERROR_500")
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
            }
            else
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            }
        }

        [SwaggerWcfTag("Books")]
        [SwaggerWcfResponse(204, "Book deleted")]
        [SwaggerWcfResponse(404, "Book not found")]
        [SwaggerWcfResponse(500, "Internal error (can be forced using ERROR_500 as book id)")]
        public void DeleteBook(string id)
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return;

            Book book = Store.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.NoContent;
                Store.Books.Remove(book);
            }
            else if (id == "ERROR_500")
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
            }
            else
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
            }
        }
    }
}
