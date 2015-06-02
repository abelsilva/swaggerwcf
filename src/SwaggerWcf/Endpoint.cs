using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using SwaggerWcf.Configuration;
using SwaggerWcf.Models;
using SwaggerWcf.Support;

namespace SwaggerWcf
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Endpoint : IEndpoint
    {
        private static Service Service { get; set; }
        private static int _initialized;

        public Endpoint()
        {
            Init();
        }

        private static void Init()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
                return;

            Service = ServiceBuilder.Build();
        }

        public static void Configure(Info info)
        {
            Init();
            Service.Info = info;
        }

        public Stream GetSwagger()
        {
            WebOperationContext woc = WebOperationContext.Current;
            if (woc != null)
                woc.OutgoingResponse.ContentType = "application/json";
            
            return new MemoryStream(Encoding.UTF8.GetBytes(Serializer.Process(Service)));
        }

        public Stream GetSwaggerFile()
        {
            return GetSwagger();
        }
    }
}
