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
    public class SwaggerWcfEndpoint : SwaggerWcfEndpointBase
    {
        private static Service Service { get; set; }
        private static String _lastLocalPath = "";
        private static Dictionary<string, List<string>> _routePrefix_VisibleTag;
        private static Info _info;
        private static SecurityDefinitions _securityDefinitions;


        public SwaggerWcfEndpoint()
        {
            Init();
        }

        private static void Init()
        {

            String localPath = GetLocalPath();

            //Only StaticContent
            try
            {
                if (!System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.Data.Equals("StaticContent"))
                    return;
            }
            catch { }

            if (localPath != "" &&_lastLocalPath.Equals(localPath))
                return;

            _lastLocalPath = localPath;

            Service = ServiceBuilder.Build(Get_VisibleTags(localPath));

            Service.Info = _info;
            Service.SecurityDefinitions = _securityDefinitions;            
        }

        private static string GetLocalPath()
        {
            String localPath = "";

            try
            {
                localPath = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri.LocalPath;
            }
            catch { }

            return localPath;
        }
     
        public static void Add_VisibleTag(String routePrefix, String tagName)
        {
            if (_routePrefix_VisibleTag == null)
                _routePrefix_VisibleTag = new Dictionary<string, List<string>>();

            if (!routePrefix.StartsWith("/"))
                routePrefix = "/" + routePrefix;

            if (routePrefix.EndsWith("/"))
                routePrefix = routePrefix.Substring(routePrefix.Length - 1);

            if (_routePrefix_VisibleTag.TryGetValue(routePrefix, out List<string> visibleTags) == false)
            {
                visibleTags = new List<string>();
                _routePrefix_VisibleTag.Add(routePrefix, visibleTags);
            }

            visibleTags.Add(tagName);
        }

        private static List<string> Get_VisibleTags(String routePrefix)
        {
            if (_routePrefix_VisibleTag == null)
                return null;

            _routePrefix_VisibleTag.TryGetValue(routePrefix, out List<string> visibleTags);

            return visibleTags;
        }

        public static void Configure(Info info, SecurityDefinitions securityDefinitions = null)
        {
            _info = info;
            _securityDefinitions = securityDefinitions;

            Init();
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
