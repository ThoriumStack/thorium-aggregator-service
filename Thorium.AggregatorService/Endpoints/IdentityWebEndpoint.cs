using Serilog;
using Thorium.Aggregator.RestServices;
using Thorium.Core.MicroServices.ConfigurationModels;
using Thorium.Core.MicroServices.Restful;

namespace Thorium.Aggregator.Endpoints
{
    internal class IdentityWebEndpoint : DefaultWebServiceEndpoint<IdentityRestService>
    {

        public IdentityWebEndpoint(ILogger logger, WebServiceSettings webServiceSettings) :
            base (logger, webServiceSettings)
        { }

        public override string EndpointDescription => "Identity Web Service Endpoint";
    }
}