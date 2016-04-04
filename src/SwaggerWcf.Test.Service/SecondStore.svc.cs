using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;

namespace SwaggerWcf.Test.Service
{
    [ServiceContract]
    [SwaggerWcf("/v2/rest")]
    public class SecondStore
    {
        [SwaggerWcfPath("Get books", "Retrieve all books from the store")]
        [WebGet(UriTemplate = "/books", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        [SwaggerWcfTag("Books")]
        [SwaggerWcfResponse(HttpStatusCode.OK, "Books found, values in the response body")]
        [SwaggerWcfResponse(HttpStatusCode.NotFound, "Book not found", true)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError,
            "Internal error (can be forced using ERROR_500 as book id)", true)]
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
    }
}
