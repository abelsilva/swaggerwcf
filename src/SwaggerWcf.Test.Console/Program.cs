using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerWcf.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:9000/wcf";
            try
            {
                WebHost(uri);

                SetupSwagger(uri);

                System.Console.ReadLine();
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        static void WebHost(string uri)
        {
            ServiceHost host = new WebServiceHost(typeof(Service1), new Uri(uri));
            var endpoint = host.AddServiceEndpoint(typeof(IService1), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior() { HelpEnabled = true });

            host.Opened += (x, y) => { System.Console.WriteLine("host open"); };
            host.Open();
        }

        static void SetupSwagger(string uri)
        {
            var host = new WebServiceHost(typeof(SwaggerWcfEndpoint), new Uri(uri));
            host.AddServiceEndpoint(typeof(ISwaggerWcfEndpoint), new WebHttpBinding(), "docs");
            try
            {
                host.Open();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }
    }
}
