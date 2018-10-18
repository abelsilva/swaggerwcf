using SwaggerWcf.Test.Service.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SwaggerWcf.Test.Service
{
    public abstract class BaseService<T> : IBaseService
    {
        public virtual string TestService(string input)
        {
            return $"input = {input}. Now = {DateTime.Now}";
        }

        public string TestServicePost(string input, string postObj)
        {
            return $"input = {input}. Now = {DateTime.Now}";
        }
    }
}