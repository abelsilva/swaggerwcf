using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerWcf.Test.Service
{
    [ServiceContract]
    public interface IBaseService
    {
        [SwaggerWcfPath("Test Service")]
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "testService?input={input}")]
        string TestService(string input);

        [SwaggerWcfPath("Test Service")]
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "testServicePost?input={input}")]
        string TestServicePost(string input, string postObj);
    }
}
