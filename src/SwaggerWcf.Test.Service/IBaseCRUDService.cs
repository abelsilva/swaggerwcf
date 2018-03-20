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
        [SwaggerWcfPath("Get Operation")]
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "get/{id}")]
        T Get(string id);

        [SwaggerWcfPath("Create Operation")]
        [OperationContract]
        [WebInvoke(Method = "PUT",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "")]
        T Create(T item);

        [SwaggerWcfPath("Update Operation")]
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "{id}")]
        T Update(string id, T item);

        [SwaggerWcfPath("Delete Operation")]
        [OperationContract]
        [WebInvoke(Method = "DELETE",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "{id}")]
        T Delete(string id);
    }
}
