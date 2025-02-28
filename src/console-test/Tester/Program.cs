using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Azure.Security.KeyVault.Secrets;

class Program
{
    static async Task Main(string[] args)
    {
        string keyVaultName = "red-device-kv";
        string eventHubNamespace = "red-device-ehns";
        string eventHubName = "wheresmyhusband";
        string secretName = "eventHubSendToken";

        // Build the URI for the Key Vault
        var kvUri = $"https://{keyVaultName}.vault.azure.net";

        // Create a SecretClient using DefaultAzureCredential
        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

        // Retrieve the secret from Key Vault
        KeyVaultSecret secret = await client.GetSecretAsync(secretName);
        string connectionString = secret.Value;

        // Create an Event Hub producer client
        await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
        {
            // Create a batch of events
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            // JSON payload
            string jsonPayload = @"
            {
              ""sensor"": {
                ""accel"": {
                  ""x"": -0.00098,
                  ""y"": 0.02989,
                  ""z"": -0.80801
                },
                ""weather"": {
                  ""co2"": 500,
                  ""hum"": 25.975053,
                  ""iaq"": 50,
                  ""pre"": 99210,
                  ""tem"": 22.045312,
                  ""voc"": 0.499999
                }
              }
            }";

            // Add events to the batch
            eventBatch.TryAdd(new EventData(new BinaryData(jsonPayload)));

            // Send the batch of events to the Event Hub
            await producerClient.SendAsync(eventBatch);
            Console.WriteLine("Message sent to Event Hub.");
        }
    }
}
