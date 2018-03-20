using SwaggerWcf.Test.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace SwaggerWcf.Test.Service
{
    [ServiceContract]
    public interface IAuthorService : IBaseService, IBaseCRUDService<Author>
    {
    }
}