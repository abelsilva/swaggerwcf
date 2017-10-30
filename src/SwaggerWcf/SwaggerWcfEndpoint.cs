using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using SwaggerWcf.Models;
using SwaggerWcf.Support;

namespace SwaggerWcf
{
    public delegate Stream GetFileCustomDelegate(string filename, out string contentType, out long contentLength);

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SwaggerWcfEndpoint : SwaggerWcfEndpointBase
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
            
            Service = ServiceBuilder.Build(paths);
            Service.Info = Info;
            Service.SecurityDefinitions = SecurityDefinitions;
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
