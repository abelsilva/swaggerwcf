using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
    public class SwaggerWcfEndpoint : ISwaggerWcfEndpoint
    {
        private static Service Service { get; set; }
        private static int _initialized;

        public SwaggerWcfEndpoint()
        {
            Init();
        }

        private static void Init()
        {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0)
                return;

            Service = ServiceBuilder.Build();
        }

        public static void SetCustomZip(Stream customSwaggerUiZipStream)
        {
            if (customSwaggerUiZipStream != null)
                Support.StaticContent.SetArchiveCustom(customSwaggerUiZipStream);
        }

        public static void SetCustomGetFile(GetFileCustomDelegate getFileCustom)
        {
            Support.StaticContent.GetFileCustom = getFileCustom;
        }

        public static void Configure(Info info, SecurityDefinitions securityDefinitions = null)
        {
            Init();
            Service.Info = info;
            Service.SecurityDefinitions = securityDefinitions;
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
                string swaggerUrl = woc.IncomingRequest.UriTemplateMatch.BaseUri.LocalPath + "/swagger.json";
                woc.OutgoingResponse.StatusCode = HttpStatusCode.Redirect;
                woc.OutgoingResponse.Location = "index.html?url=" + swaggerUrl;
                return null;
            }

            string filename = content.Contains("?")
                ? content.Substring(0, content.IndexOf("?", StringComparison.Ordinal))
                : content;

            string contentType;
            long contentLength;
            Stream stream = Support.StaticContent.GetFile(filename, out contentType, out contentLength);

            if (stream == Stream.Null)
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

            woc.OutgoingResponse.StatusCode = HttpStatusCode.OK;
            woc.OutgoingResponse.ContentLength = contentLength;
            woc.OutgoingResponse.ContentType = contentType;

            return stream;
        }
    }
}
