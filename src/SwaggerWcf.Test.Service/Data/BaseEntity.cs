using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SwaggerWcf.Test.Service.Data
{
    public abstract class BaseEntity
    {
        [DataMember]
        public virtual string Id { get; set; }
    }
}