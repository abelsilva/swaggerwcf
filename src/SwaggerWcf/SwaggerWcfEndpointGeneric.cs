using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using SwaggerWcf.Models;
using SwaggerWcf.Support;

namespace SwaggerWcf
{
    public class SwaggerWcfEndpoint<TBusiness> : SwaggerWcfEndpointBase
    {
        private static Service Service { get; set; }
        public static Info Info { get; private set; }
        public static SecurityDefinitions SecurityDefinitions { get; private set; }

        private static int _initialized;

        public SwaggerWcfEndpoint()
        {
            Init();
        }

        private static void Init()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
                return;

            string[] paths = OperationContext.Current?.Host.BaseAddresses.Select(ba => ba.AbsolutePath).ToArray();

            Service = ServiceBuilder.Build<TBusiness>(paths);
        }

        public static void Configure(Info info, SecurityDefinitions securityDefinitions = null)
        {
            Info = info;
            SecurityDefinitions = securityDefinitions;
        }

        public override Stream GetSwaggerFile()
        {
            WebOperationContext woc = WebOperationContext.Current;
            if (woc != null)
            {
                //TODO: create a parameter in settings to configure this
                woc.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                woc.OutgoingResponse.ContentType = "application/json";
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(Serializer.Process(Service)));
        }
    }
}
