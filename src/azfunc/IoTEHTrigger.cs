using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace wheresmydevice.function
{
    public class IoTEHTrigger
    {
        private readonly ILogger<IoTEHTrigger> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public IoTEHTrigger(ILogger<IoTEHTrigger> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer("wheresmydevice", "IoTData");
        }

        [Function(nameof(IoTEHTrigger))]
        public void Run([EventHubTrigger("wheresmyhusband", Connection = "reddeviceehns_ListenAccessKey_EVENTHUB")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                _logger.LogInformation("Event Body: {body}", @event.Body);
                _logger.LogInformation("Event Content-Type: {contentType}", @event.ContentType);
            }
        }
    }
}
