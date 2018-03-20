using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using SwaggerWcf.Models;
using SwaggerWcf.Support;
using System.Runtime.CompilerServices;

namespace SwaggerWcf
{
    public delegate Stream GetFileCustomDelegate(string filename, out string contentType, out long contentLength);

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SwaggerWcfEndpoint : ISwaggerWcfEndpoint
    {
        public SwaggerWcfEndpoint()
        {
            Init(ServiceBuilder.Build);
        }

        internal SwaggerWcfEndpoint(Func<string, Service> buildService)
        {
            Init(buildService);
        }

        internal static Info Info { get; private set; }

        internal static SecurityDefinitions SecurityDefinitions { get; private set; }

        internal static string BasePath { get; private set; }

        private static Dictionary<string, string> SwaggerFiles { get; } = new Dictionary<string, string>();

        public static bool DisableSwaggerUI { get; set; }

        public static Func<string, List<string>, List<string>> FilterVisibleTags { get; set; } =
            (string path, List<string> visibleTags) => visibleTags;

        public static Func<string, List<string>, List<string>> FilterHiddenTags { get; set; } =
            (string path, List<string> hiddenTags) => hiddenTags;

        public static void Configure(Info info, SecurityDefinitions securityDefinitions = null, string basePath = null)
        {
            Info = info;
            SecurityDefinitions = securityDefinitions;
            BasePath = basePath;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static void Init(Func<string, Service> buildService)
        {
            string[] paths = GetAllPaths().Where(p => !SwaggerFiles.Keys.Contains(p)).ToArray();

            foreach (string path in paths)
            {
                Service service = buildService(path);
                if (Info != null)
                    service.Info = Info;
                if (SecurityDefinitions != null)
                    service.SecurityDefinitions = SecurityDefinitions;
                if (BasePath != null)
                    service.BasePath = BasePath;

                string swagger = Serializer.Process(service);
                if (SwaggerFiles.ContainsKey(path) == false)
                    SwaggerFiles.Add(path, swagger);
            }
        }

        private static string[] GetAllPaths()
        {
            return OperationContext.Current?.Host?.BaseAddresses.Select(ba => ba.AbsolutePath).ToArray()
                   ?? new[] { "" };
        }

        private static string GetSwaggerFileContents()
        {
            string fullPath = WebOperationContext.Current?.IncomingRequest.UriTemplateMatch?.RequestUri?.AbsolutePath ?? "";
            string key = SwaggerFiles.Keys.ToList().ClosestMatch(fullPath);

            return key != null ? SwaggerFiles[key] : "";
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

        public Stream GetSwaggerFile()
        {
            WebOperationContext woc = WebOperationContext.Current;
            if (woc != null)
            {
                //TODO: create a parameter in settings to configure this
                woc.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
                woc.OutgoingResponse.ContentType = "application/json";
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(GetSwaggerFileContents()));
        }

        public Stream StaticContent(string content)
        {
            WebOperationContext woc = WebOperationContext.Current;

            if (woc == null)
                return Stream.Null;

            if (DisableSwaggerUI)
            {
                woc.OutgoingResponse.StatusCode = HttpStatusCode.NotFound;
                return null;
            }

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
