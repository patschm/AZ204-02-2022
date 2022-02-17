using Azure.Storage.Queues;
using System;
using System.Threading.Tasks;

namespace StorageQueueReader
{
    class Program
    {
        static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=Hr/9y39UMzihBa1zD+FyudPw5XV/Z82QUGAz5jFc0b25YYcffNOsxq0mpdj7BdHHJUpWqdmj6pZFJb/osLQHWw==;EndpointSuffix=core.windows.net";
        static string QueueName = "the-queue";
        static async Task Main(string[] args)
        {
            await ReadFromQueueAsync();
            Console.WriteLine("Press Enter to Quit");
            Console.ReadLine();
        }

        private static async Task ReadFromQueueAsync()
        {
            var client = new QueueClient(ConnectionString, QueueName);
            do
            {
                // 10 seconds "lease" time
                var response = await client.ReceiveMessageAsync(TimeSpan.FromSeconds(10));
                if (response.Value == null)
                {
                    await Task.Delay(100);
                    continue;
                }
                var msg = response.Value;
                Console.WriteLine(msg.Body.ToString());

                // We need more time to process
                //await client.UpdateMessageAsync(msg.MessageId, msg.PopReceipt, msg.Body, TimeSpan.FromSeconds(30));
                // Don't forget to remove
                await client.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
            }
            while (true);
        }
    }
}
