using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

namespace SwaggerWcf.Test.Service.WCF
{
    public class WebServiceHostFactoryEx : WebServiceHostFactory
    {
        protected Type interfaceType = null;
        protected ServiceHost m_serviceHost = null;
        protected Type m_serviceType = null;
        protected Uri[] m_baseAddresses = null;

        public WebServiceHostFactoryEx(Type interfaceType)
        {
            this.interfaceType = interfaceType;
        }
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            if (m_serviceHost == null)
            {
                m_serviceType = serviceType;
                m_baseAddresses = baseAddresses;
                m_serviceHost = CreateNormalWCFServiceHost(serviceType, baseAddresses);
            }
            return m_serviceHost;
        }

        protected virtual ServiceHost CreateNormalWCFServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost serviceHost = base.CreateServiceHost(serviceType, baseAddresses);

            //Add REST Service Endpoint
            {
                //HTTP
                var httpBaseAddress = serviceHost.BaseAddresses.Where(a => a.Scheme == Uri.UriSchemeHttp).FirstOrDefault();
                if (httpBaseAddress != null)
                {
                    var httpBinding = new WebHttpBinding();
                    var serviceEndpoint = serviceHost.AddServiceEndpoint(interfaceType, httpBinding, "");
                    serviceEndpoint.Name = "HttpEndpoint";
                    //Help Page Enable for HTTP
                    serviceEndpoint.EndpointBehaviors.Add(new WebHttpBehavior { HelpEnabled = true, FaultExceptionEnabled = true });
                }
                //HTTPS
                var httpsBaseAddress = serviceHost.BaseAddresses.Where(a => a.Scheme == Uri.UriSchemeHttps).FirstOrDefault();
                if (httpsBaseAddress != null)
                {
                    var httpsBinding = new WebHttpBinding(WebHttpSecurityMode.Transport);
                    var serviceEndpoint = serviceHost.AddServiceEndpoint(interfaceType, httpsBinding, "");
                    serviceEndpoint.Name = "HttpsEndpoint";
                    //Help Page Enable
                    serviceEndpoint.EndpointBehaviors.Add(new WebHttpBehavior { HelpEnabled = true, FaultExceptionEnabled = true });
                }
            }
            addMetadataEndpoint(ref serviceHost);

            return serviceHost;
        }
        

        protected void addMetadataEndpoint(ref ServiceHost serviceHost)
        {
            // Check to see if the service host already has a ServiceMetadataBehavior
            ServiceMetadataBehavior smb = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            // If not, add one
            if (smb == null)
            {
                smb = new ServiceMetadataBehavior { HttpGetEnabled = true, HttpsGetEnabled = true };
                serviceHost.Description.Behaviors.Add(smb);
            }
            
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
        }        
    }
}
