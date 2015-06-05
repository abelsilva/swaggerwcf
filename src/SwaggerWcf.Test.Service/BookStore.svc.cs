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
        [Tag("Books")]
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
        
        [Tag("Books")]
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
        
        [Tag("Books")]
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
        
        [Tag("Books")]
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
        
        [Tag("Books")]
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
