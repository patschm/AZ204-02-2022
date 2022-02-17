using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queue
{
    class Program
    {
        private static string storage = "psthestoor";
        private static string key = "bWM/MSLkC5dEwQajPbGm4V2kZ0TPgYcHPRWGBjFgr6S1Grddbxr7+Cw1A292TzWFqEFrHhsuXAstsqJsGwdYlQ==";

        static void Main(string[] args)
        {
            //CreateQueue();
            //Enqueue();
            Dequeue();
            //DeleteQueue();
            Console.WriteLine("Ready");
            Console.ReadLine();
        }

        private static void CreateQueue()
        {
            CloudStorageAccount account = CreateAccount();
            CloudQueueClient client = account.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("myqueue");
            queue.CreateIfNotExists();
        }

        private static void Enqueue()
        {
            CloudStorageAccount account = CreateAccount();
                //CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=psthestoor;AccountKey=bWM/MSLkC5dEwQajPbGm4V2kZ0TPgYcHPRWGBjFgr6S1Grddbxr7+Cw1A292TzWFqEFrHhsuXAstsqJsGwdYlQ==;EndpointSuffix=core.windows.net");
            
            CloudQueueClient client = account.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("test-q");
            //queue.CreateIfNotExists();

            
            for (int i = 0; i < 100; i++)
            {
                CloudQueueMessage message = new CloudQueueMessage("Hello World " + i);
                queue.AddMessage(message);
            }
        }

        private static void Dequeue()
        {
            CloudStorageAccount account = CreateAccount();
            CloudQueueClient client = account.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("test-q");
            //queue.CreateIfNotExists();

            CloudQueueMessage message = null;
            do
            {
                message = queue.GetMessage(TimeSpan.FromSeconds(10));
                if (message == null) break;
                Console.WriteLine(message.AsString);
                queue.DeleteMessage(message);
            }
            while (true);
            
        }

        private static void DeleteQueue()
        {
            CloudStorageAccount account = CreateAccount();
            CloudQueueClient client = account.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference("myqueue");
            queue.DeleteIfExists();
        }

        private static CloudStorageAccount CreateAccount()
        {
            string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storage, key);
            CloudStorageAccount account;
            if (CloudStorageAccount.TryParse(connectionString, out account))
            {
                return account;
            }
            return null;
        }
    }
}
