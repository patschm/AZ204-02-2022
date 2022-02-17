using System;
using System.Text;
using System.Threading.Tasks;

// New
using NewNS= Azure.Messaging.EventHubs;


namespace EvtHubClient
{
    class Program
    {
        private static string conStr = "Endpoint=sb://ps-evt-hubs.servicebus.windows.net/;SharedAccessKeyName=Writer;SharedAccessKey=/EFM/nXiliGx/Yr0TK2dt7HFX8RwMEf6bFa+OU9waRo=;EntityPath=een-hub";
        private static string hubName = "een-hub";

        static async Task Main(string[] args)
        {
            // Check!! AZ-204 book describes EventHubClient which is an obsolete package
            // Use this solutuin
            await NewStyle();
            
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static async Task NewStyle()
        {
            await using (var producerClient = new NewNS.Producer.EventHubProducerClient(conStr, hubName))
            {
                var eventBatch = await producerClient.CreateBatchAsync();
                for (int i = 0; i < 200; i++)
                {
                    eventBatch.TryAdd(new NewNS.EventData(Encoding.UTF8.GetBytes($"Hello World {i}")));
                }
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("Sent");
            }         
        }
    }
}
