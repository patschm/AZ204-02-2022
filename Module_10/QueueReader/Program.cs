using System;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.ServiceBus;

namespace QueueReader
{
    class Program
    {
        static string EndPoint = "ps-zeurbus.servicebus.windows.net";
        static (string Name, string KeY) SasKeyReader = ("Reader", "hdmIdoL3WVIKH2+05gr/X6lNpHWkFV0XG5mx3hzuemk=");
        static string QueueName = "ze-queue";

        static async Task Main(string[] args)
        {
            //await ReadQueueAsync();
            await ReadQueueProcessorAsync();
            Console.WriteLine("Press Enter to Quit");
            Console.ReadLine();
        }

        private static async Task ReadQueueAsync()
        {
            var cred = new AzureNamedKeyCredential(SasKeyReader.Name, SasKeyReader.KeY);
            var client = new ServiceBusClient(EndPoint, cred);
            var receiver = client.CreateReceiver(QueueName);
            do
            {
                var msg = await receiver.ReceiveMessageAsync();
                Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken}");
                var data = msg.Body.ToString();
                Console.WriteLine(data);
                await receiver.CompleteMessageAsync(msg);
                //await receiver.AbandonMessageAsync(msg);
                //await receiver.RenewMessageLockAsync(msg);
                await Task.Delay(1000);
            }
            while (true);
        }
        private static async Task ReadQueueProcessorAsync()
        {
            var cred = new AzureNamedKeyCredential(SasKeyReader.Name, SasKeyReader.KeY);
            var client = new ServiceBusClient(EndPoint, cred);
            var receiver = client.CreateProcessor(QueueName);
            
            receiver.ProcessMessageAsync += async evtArg => {
                var msg = evtArg.Message;
                Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken}");
                var data = msg.Body.ToString();
                Console.WriteLine(data);
                await evtArg.CompleteMessageAsync(msg);
            };

            receiver.ProcessErrorAsync += evtArg => {
                Console.WriteLine("Ooops");
                Console.WriteLine(evtArg.Exception.Message);
                return Task.CompletedTask;
            };

            await receiver.StartProcessingAsync();
            Console.WriteLine("Press Enter to quit processing");
            Console.ReadLine();
            await receiver.StopProcessingAsync();

        }
    }
}
