using Azure.Storage.Queues;
using System;
using System.Threading.Tasks;

namespace StorageQueueWriter
{
    class Program
    {
        static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=Hr/9y39UMzihBa1zD+FyudPw5XV/Z82QUGAz5jFc0b25YYcffNOsxq0mpdj7BdHHJUpWqdmj6pZFJb/osLQHWw==;EndpointSuffix=core.windows.net";
        static string QueueName = "the-queue";
        static async Task Main(string[] args)
        {
            await WriteToQueueAsync();
            Console.WriteLine("Press Enter to Quit");
            Console.ReadLine();
        }

        private static async Task WriteToQueueAsync()
        {
            var client = new QueueClient(ConnectionString, QueueName);
            for (int i = 0; i < 100; i++)
            {
                await client.SendMessageAsync($"Hello Number {i}");
            }
        }

    }
}
