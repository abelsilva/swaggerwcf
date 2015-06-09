using System;
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
        private static Service Service
        { get; set; }
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

        public Stream GetSwaggerFile()
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

        public Stream StaticContent(string content)
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return Stream.Null;

            if (string.IsNullOrWhiteSpace(content))
            {
                string swaggerUrl = woc.IncomingRequest.UriTemplateMatch.BaseUri + "/swagger.json";
                woc.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Redirect;
                woc.OutgoingResponse.Location = "index.html?url=" + swaggerUrl;
                return null;
            }

            string filename = content.Contains("?") ? content.Substring(0, content.IndexOf("?", StringComparison.Ordinal)) : content;

            string contentType;
            long contentLength;
            var stream = Support.StaticContent.GetFile(filename, out contentType, out contentLength);

            if (stream == Stream.Null)
            {
                woc.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                return null;
            }

            woc.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
            woc.OutgoingResponse.ContentLength = contentLength;
            woc.OutgoingResponse.ContentType = contentType;

            return stream;
        }
    }
}