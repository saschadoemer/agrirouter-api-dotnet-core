using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Agrirouter.Api.Definitions;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Api.Service.Parameters.Inner;
using Agrirouter.Api.Test.Data;
using Agrirouter.Api.Test.Helper;
using Agrirouter.Impl.Service.Common;
using Agrirouter.Impl.Service.Messaging;
using Agrirouter.Request.Payload.Endpoint;
using Xunit;

namespace Agrirouter.Api.Test.Service.Messaging.Http
{
    /// <summary>
    ///     Functional tests.
    /// </summary>
    [Collection("Integrationtest")]
    public class CapabilitiesServiceTest : AbstractIntegrationTestForCommunicationUnits
    {
        private static readonly HttpClient HttpClient = HttpClientFactory.AuthenticatedHttpClient(OnboardResponse);

        private static OnboardResponse OnboardResponse =>
            OnboardResponseIntegrationService.Read(Identifier.Http.CommunicationUnit.SingleEndpointWithoutRoute);

        [Fact]
        public void GivenValidCapabilitiesWhenSendingCapabilitiesMessageThenTheAgrirouterShouldSetTheCapabilities()
        {
            var capabilitiesServices =
                new CapabilitiesService(new HttpMessagingService(HttpClient));
            var capabilitiesParameters = new CapabilitiesParameters
            {
                OnboardResponse = OnboardResponse,
                ApplicationId = Applications.CommunicationUnit.ApplicationId,
                CertificationVersionId = Applications.CommunicationUnit.CertificationVersionId,
                EnablePushNotifications = CapabilitySpecification.Types.PushNotification.Disabled,
                CapabilityParameters = new List<CapabilityParameter>()
            };

            capabilitiesParameters.CapabilityParameters.AddRange(Capabilities);
            capabilitiesServices.Send(capabilitiesParameters);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            var fetchMessageService = new FetchMessageService(HttpClient);
            var fetch = fetchMessageService.Fetch(OnboardResponse);
            Assert.Single(fetch);

            var decodedMessage = DecodeMessageService.Decode(fetch[0].Command.Message);
            Assert.Equal(201, decodedMessage.ResponseEnvelope.ResponseCode);
        }

        private static IEnumerable<CapabilityParameter> Capabilities
        {
            get
            {
                var all = new List<CapabilityParameter>();
                TechnicalMessageTypes.AllForCapabilitySetting().ForEach(technicalMessageType =>
                {
                    var capabilitiesParameter = new CapabilityParameter
                    {
                        Direction = CapabilitySpecification.Types.Direction.SendReceive,
                        TechnicalMessageType = technicalMessageType
                    };
                    all.Add(capabilitiesParameter);
                });
                return all;
            }
        }
    }
}