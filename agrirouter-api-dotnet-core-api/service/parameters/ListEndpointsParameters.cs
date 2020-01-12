using Agrirouter.Request.Payload.Account;

namespace com.dke.data.agrirouter.api.service.parameters
{
    /**
     * Parameter container definition.
     */
    public class ListEndpointsParameters : MessageParameters
    {
        public ListEndpointsQuery.Types.Direction Direction { get; set; }

        public string TechnicalMessageType { get; set; }
    }
}