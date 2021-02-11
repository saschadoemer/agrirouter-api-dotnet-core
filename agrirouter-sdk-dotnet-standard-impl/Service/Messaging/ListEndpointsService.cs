using Agrirouter.Sdk.Api.Definitions;
using Agrirouter.Sdk.Api.Service.Messaging;
using Agrirouter.Sdk.Api.Service.Parameters;
using Agrirouter.Sdk.Impl.Service.Messaging.Abstraction;

namespace Agrirouter.Sdk.Impl.Service.Messaging
{
    /// <summary>
    ///     Service to list the endpoints connected to an endpoint.
    /// </summary>
    public class ListEndpointsService : ListEndpointsBaseService
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="messagingService"></param>
        public ListEndpointsService(IMessagingService<MessagingParameters> messagingService) :
            base(messagingService)
        {
        }

        protected override string TechnicalMessageType => TechnicalMessageTypes.DkeListEndpoints;
    }
}