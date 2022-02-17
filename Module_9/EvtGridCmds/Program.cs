using Azure;
using Azure.Messaging.EventGrid;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvtGridCmds
{
    class Program
    {
        static string topicHost = "ps-event-grid.westeurope-1.eventgrid.azure.net";
        static string topicEP = $"https://{topicHost}/api/events";
        static string topicKey = "reR6+0fMVzILXQ2h3EvieprJEvBJVmSJLBi3UZyZqXQ=";

        static async Task Main(string[] args)
        {
            var topicCredentials = new AzureKeyCredential(topicKey);
            EventGridPublisherClient client = new EventGridPublisherClient(new Uri(topicEP), topicCredentials);

            var events = new List<EventGridEvent>();
            ConsoleKey key;
            do
            {
                Console.WriteLine("Firing event...");
                var evtData = new Dictionary<string, string> { { "een", "42" }, { "twee", "Hello from code" } };
                events.Clear();
                var gev = new EventGridEvent("Some Subject", "Posting something", "2.0", evtData);
                
                events.Add(gev);
                await client.SendEventsAsync(events);
                Console.WriteLine("Another event? (Esc to quit)");
                key = Console.ReadKey().Key;
            }
            while (key != ConsoleKey.Escape);
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
