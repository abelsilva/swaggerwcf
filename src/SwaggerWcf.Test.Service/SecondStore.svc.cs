using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;

namespace SwaggerWcf.Test.Service
{
    [ServiceContract]
    [SwaggerWcf("/v2/rest")]
    public class SecondStore
    {
        [SwaggerWcfPath("Get book", "Retrieve a book from the store using its id")]
        [WebGet(UriTemplate = "/{id}?year={year}", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [SwaggerWcfTag("Books")]
        [SwaggerWcfResponse(HttpStatusCode.OK, "Books found, values in the response body")]
        [SwaggerWcfResponse(HttpStatusCode.NotFound, "Book not found", true)]
        [SwaggerWcfResponse(HttpStatusCode.InternalServerError,
            "Internal error (can be forced using ERROR_500 as book id)", true)]
        public Task<Book> ReadBook(string id, int year = 0)
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return null;

            Book book = Store.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                return Task.FromResult(book);
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
