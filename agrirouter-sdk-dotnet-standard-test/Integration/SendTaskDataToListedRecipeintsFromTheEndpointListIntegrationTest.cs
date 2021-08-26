using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Agrirouter.Api.Definitions;
using Agrirouter.Api.Dto.Onboard;
using Agrirouter.Api.Service.Parameters;
using Agrirouter.Impl.Service.Common;
using Agrirouter.Impl.Service.Messaging;
using Agrirouter.Request.Payload.Account;
using Agrirouter.Response;
using Agrirouter.Test.Data;
using Agrirouter.Test.Helper;
using Agrirouter.Test.Service;
using Xunit;

namespace Agrirouter.Test.Integration
{
    public class
        SendTaskDataToListedRecipeintsFromTheEndpointListIntegrationTest : AbstractIntegrationTestForCommunicationUnits
    {
        private static readonly HttpClient HttpClient = HttpClientFactory.AuthenticatedHttpClient(OnboardResponse);

        private static OnboardResponse OnboardResponse =>
            OnboardResponseIntegrationService.Read(Identifier.Http.CommunicationUnit.SenderWithMultipleRecipients);

        /**
         * Searching for all endpoints that are connected via route and are able to RECEIVE.
         */
        [Fact]
        public void
            GivenMultipleRecipientsWhenFilteringForEndpointsThatCanReceiveThenTheResultSetShouldContainTheExpectedOnes()
        {
            var listEndpointsService =
                new ListEndpointsService(new HttpMessagingService(HttpClient));
            var listEndpointsParameters = new ListEndpointsParameters
            {
                OnboardResponse = OnboardResponse,
                Direction = ListEndpointsQuery.Types.Direction.Send,
                TechnicalMessageType = TechnicalMessageTypes.Iso11783TaskdataZip
            };
            listEndpointsService.Send(listEndpointsParameters);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            var fetchMessageService = new FetchMessageService(HttpClient);
            var fetch = fetchMessageService.Fetch(OnboardResponse);
            Assert.Single(fetch);

            var decodedMessage = DecodeMessageService.Decode(fetch[0].Command.Message);
            Assert.Equal(200, decodedMessage.ResponseEnvelope.ResponseCode);
            Assert.Equal(ResponseEnvelope.Types.ResponseBodyType.EndpointsListing,
                decodedMessage.ResponseEnvelope.Type);

            var listEndpointsResponse = listEndpointsService.Decode(decodedMessage.ResponsePayloadWrapper.Details);

            // Asserting only that the number of endpoints is at least those three that are expected when searching for receivers.
            Assert.True(listEndpointsResponse.Endpoints.Count >= 2);

            const string endpointThatCanSend = "949f33a0-b758-4018-8cfd-057e7d3030b2";
            const string endpointThatCanReceive = "206b5e98-9ac8-4569-8332-742cf93f58c2";
            const string endpointThatCanSendAndReceive = "39db0f54-052e-4bf1-b5aa-654d3adf91a7";

            var endpoints = listEndpointsResponse.Endpoints
                .Where(endpoint => endpoint.EndpointId.Equals(endpointThatCanSend) ||
                                   endpoint.EndpointId.Equals(endpointThatCanReceive) ||
                                   endpoint.EndpointId.Equals(endpointThatCanSendAndReceive))
                .ToList();

            Assert.Equal(2, endpoints.Count);

            var publishAndSendMessageService = new PublishAndSendMessageService(new HttpMessagingService(HttpClient));
            var sendMessageParameters = new SendMessageParameters
            {
                OnboardResponse = OnboardResponse,
                Recipients = new List<string>(endpoints.Select(e => e.EndpointId)),
                Base64MessageContent = DataProvider.ReadBase64EncodedSmallTaskData(),
                TechnicalMessageType = TechnicalMessageTypes.Iso11783TaskdataZip
            };
            publishAndSendMessageService.Send(sendMessageParameters);

            Thread.Sleep(TimeSpan.FromSeconds(5));
            
            fetch = fetchMessageService.Fetch(OnboardResponse);
            Assert.Single(fetch);

            decodedMessage = DecodeMessageService.Decode(fetch[0].Command.Message);
            Assert.Equal(201, decodedMessage.ResponseEnvelope.ResponseCode);
            Assert.Equal(ResponseEnvelope.Types.ResponseBodyType.Ack,
                decodedMessage.ResponseEnvelope.Type);
        }
    }
}