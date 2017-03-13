using System;
using System.IO;
using System.Net;
using System.ServiceModel.Web;

namespace SwaggerWcf
{
    public abstract class SwaggerWcfEndpointBase : ISwaggerWcfEndpoint
    {
        public static void SetCustomZip(Stream customSwaggerUiZipStream)
        {
            if (customSwaggerUiZipStream != null)
                Support.StaticContent.SetArchiveCustom(customSwaggerUiZipStream);
        }

        public static void SetCustomGetFile(GetFileCustomDelegate getFileCustom)
        {
            Support.StaticContent.GetFileCustom = getFileCustom;
        }

        public abstract Stream GetSwaggerFile(); 

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
