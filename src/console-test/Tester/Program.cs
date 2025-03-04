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

            // JSON payloads
            string[] jsonPayloads = new string[]
            {
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7767,
                            ""lon"": -96.7970,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7768,
                            ""lon"": -96.7971,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7769,
                            ""lon"": -96.7972,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7770,
                            ""lon"": -96.7973,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7771,
                            ""lon"": -96.7974,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7772,
                            ""lon"": -96.7975,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7773,
                            ""lon"": -96.7976,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7774,
                            ""lon"": -96.7977,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7775,
                            ""lon"": -96.7978,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }",
                @"{
                    ""sensor"": {
                        ""accel"": {
                            ""x"": -0.008085,
                            ""y"": 0.0294,
                            ""z"": -0.803845
                        },
                        ""weather"": {
                            ""co2"": 467.279876,
                            ""hum"": 30.058282,
                            ""iaq"": 35,
                            ""pre"": 98511,
                            ""tem"": 20.995311,
                            ""voc"": 0.43105
                        },
                        ""gnss"": {
                            ""timeBootMs"": 600681604,
                            ""lat"": 32.7776,
                            ""lon"": -96.7979,
                            ""alt"": -91020,
                            ""relativeAlt"": -86482,
                            ""vx"": -2,
                            ""vy"": 5,
                            ""vz"": 2,
                            ""hdg"": 25473
                        }
                    }
                }"
            };

            // Add events to the batch
            foreach (var payload in jsonPayloads)
            {
                eventBatch.TryAdd(new EventData(new BinaryData(payload)));
            }

            // Send the batch of events to the Event Hub
            await producerClient.SendAsync(eventBatch);
            Console.WriteLine("Messages sent to Event Hub.");
        }
    }
}
