using System.ServiceModel.Activation;
using SwaggerWcf.Support;

namespace SwaggerWcf
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SwaggerWcfEndpoint<TBusiness> : SwaggerWcfEndpoint
    {
        public SwaggerWcfEndpoint() : base(ServiceBuilder.Build<TBusiness>)
        {
        }
    }
}