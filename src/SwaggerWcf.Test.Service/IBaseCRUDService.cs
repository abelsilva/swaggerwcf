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
    public interface IBaseCRUDService<T>
    {
        [SwaggerWcfPath("Get")]
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "get/{id}")]
        T Get(string id);
    }
}
