using Bogus;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDemo
{
    static class ChangeFeedTest
    {
        public class Product
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            public string Name { get; set; }           
            public string Brand { get; set; }
            public string BrandWebSite { get; set; }

            [JsonProperty(PropertyName = "pkey")]
            public string ThePartitionKey { get; set; } 
        }

        private static string Host = "https://ps-cosmo.documents.azure.com:443/";
        private static string PrimaryKey = "yfu0f4XkaRTIm59lfz5ZL1EJYMbXU8h9Y4cWDD78XQ6xeMdpaf6XULBKpsuHkcpJ9nnaYuGPedr2l0HWxxh3jQ==";
        private static string Database = "mijndb";
        private static string Container = "products";

        public static async Task RunCoreSql()
        {
            var client = CreateCoreSqlClient();
            PopulateData(client);
            await SetupChangeFeedProcessorAsync(client);
            Console.WriteLine("Done");
        }
        private static CosmosClient CreateCoreSqlClient()
        {
            return new CosmosClient(Host, PrimaryKey);
        }
        private static void PopulateData(CosmosClient client)
        {
            var data = new Faker<Product>()
                .RuleFor(p => p.Id, fk => fk.UniqueIndex.ToString())
                .RuleFor(p => p.Name, fk => fk.Commerce.ProductName())
                .RuleFor(p => p.Brand, fk => fk.Commerce.Department())
                .RuleFor(p => p.BrandWebSite, fk => fk.Internet.UrlWithPath("https"))
                .RuleFor(p=> p.ThePartitionKey, (fk, p)=> $"{p.Brand}")
                .Generate(10);
            var prods = data.ToList();
            Container products = client.GetContainer(Database, Container);
            prods.ForEach(async p => await products.CreateItemAsync(p, new PartitionKey(p.ThePartitionKey)));
            Console.WriteLine("Data Created");
        }
        private static async Task SetupChangeFeedProcessorAsync(CosmosClient client)
        {
            // The lease container acts as a state storage and coordinates processing the change feed across multiple workers.
            // The lease container can be stored in the same account as the monitored container or in a separate account.
            ContainerProperties props = new ContainerProperties("leasecontainer", "/id");
            var leaseContainer = await client.GetDatabase(Database).CreateContainerIfNotExistsAsync(props, throughput: 400);
            
            Container container = client.GetContainer(Database, Container);
            // Setup the Change feed processor to listen for changes.
            // The code that is run when changes are detected is called the delegate
            var builder = container.GetChangeFeedProcessorBuilder<Product>("changes",  
                (IReadOnlyCollection<Product> roc, CancellationToken ct) => {
                    // The delegate
                    Console.WriteLine($"{roc.Count} Received");
                    foreach(var item in roc)
                    {
                        Console.WriteLine($"{item.Brand} {item.Name} ({item.BrandWebSite})");
                    }
                    return null;
            });

            // Since multiple hosts can run a change feed processors, make sure the instance name is unique
            var proc = builder
                .WithInstanceName("unique_name")
                .WithLeaseContainer(leaseContainer)
                .Build();

            await proc.StartAsync();
            Console.WriteLine("Press enter to stop");
            Console.ReadLine();
            await proc.StopAsync();

        }
    }
}
