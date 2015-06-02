using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SwaggerWcf
{
    [ServiceContract]
    internal interface IEndpoint
    {
        [OperationContract]
        [WebGet(UriTemplate = "/", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        Stream GetSwagger();

        [OperationContract]
        [WebGet(UriTemplate = "/swagger.json", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        Stream GetSwaggerFile();
    }
}
