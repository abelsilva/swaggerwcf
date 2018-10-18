using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SwaggerWcf
{
    [ServiceContract]
    public interface ISwaggerWcfEndpoint
    {
        [OperationContract]
        [WebGet(UriTemplate = "/swagger.json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        Stream GetSwaggerFile();

        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/{*content}")]
        //[WebGet(UriTemplate = "/", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        Stream StaticContent(string content);
    }
}
