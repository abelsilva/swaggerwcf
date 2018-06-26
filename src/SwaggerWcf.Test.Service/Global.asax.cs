using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using SwaggerWcf.Models;

namespace SwaggerWcf.Test.Service
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            SwaggerWcfEndpoint.FilterVisibleTags = FilterVisibleTags;
            SwaggerWcfEndpoint.FilterHiddenTags = FilterHiddenTags;
            SwaggerWcfEndpoint.DisableSwaggerUI = false;

            // route with a simple class
            RouteTable.Routes.Add(new ServiceRoute("v1/rest", new WebServiceHostFactory(), typeof(BookStore)));

            // route with inherited methods on a base class
            RouteTable.Routes.Add(new ServiceRoute("v1/authors", new WebServiceHostFactory(), typeof(AuthorService)));
            
            RouteTable.Routes.Add(new ServiceRoute("api-docs", new WebServiceHostFactory(), typeof(SwaggerWcfEndpoint)));
        }

        private static List<string> FilterVisibleTags(string path, List<string> visibleTags)
        {
            return visibleTags;
        }

        private static List<string> FilterHiddenTags(string path, List<string> hiddenTags)
        {
            return hiddenTags;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}
