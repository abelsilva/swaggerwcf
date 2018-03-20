using SwaggerWcf.Attributes;
using SwaggerWcf.Test.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerWcf.Test.Service
{
    public class BaseCRUDService<T> : BaseService<T>, IBaseCRUDService<T> where T : BaseEntity
    {
        public virtual T Create(T item)
        {
            return item;
        }

        public virtual T Delete(string id)
        {
            var result = default(T);
            result.Id = id;
            return result;
        }

        public virtual T Get(string id)
        {
            var result = default(T);
            result.Id = id;
            return result;
        }

        public virtual T Update(string id, T item)
        {
            var result = item;
            result.Id = id;
            return result;
        }
    }
}
