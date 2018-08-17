using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerWcf.Test.Console
{
    [SwaggerWcf("wcf")]
    [SwaggerWcfServiceInfo(title: "Service1", version: "1.0.0")]
    public class Service1 : IService1
    {
        [SwaggerWcfTag("MyService")]
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        [SwaggerWcfTag("MyService")]
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
