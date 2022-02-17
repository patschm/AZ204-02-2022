using Gremlin.Net.Process.Traversal;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using Repository.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDemo
{
    static class CoreSqlTest
    {
        public class Product
        {
            public string Name { get; set; }
            public string Brand { get; set; }
            public string BrandWebSite { get; set; }
        }
        public class ProductGroup
        {
            [JsonProperty(PropertyName = "id")]
            public string ID { get; set; }
            [JsonProperty(PropertyName = "pkey")]
            public string PKey { get; set; }
            public string Name { get; set; }

            public Product[] Products { get; set; }
        }


        private static string Host = "https://ps-cosmo.documents.azure.com:443/";
        private static string PrimaryKey = "yfu0f4XkaRTIm59lfz5ZL1EJYMbXU8h9Y4cWDD78XQ6xeMdpaf6XULBKpsuHkcpJ9nnaYuGPedr2l0HWxxh3jQ==";
        private static string Database = "mijndb";
        private static string Container = "products";
        public static int Port = 443;
        public static bool EnableSSL = true;

        public static async Task RunCoreSql()
        {
            var client = CreateCoreSqlClient();
            //await AddProductGroups(client);
            await ReadData(client);
            Console.WriteLine("Done");

        }
        private static CosmosClient CreateCoreSqlClient()
        {
            return new CosmosClient(Host, PrimaryKey);
        }
        
        private static async Task ReadData(CosmosClient client)
        {
            var query = "SELECT * FROM p WHERE STARTSWITH(p.Name, 'D')";
            var pContainer = client.GetContainer(Database, Container);
            var qDef = new QueryDefinition(query);
            string continuationToken = null;

            FeedIterator<ProductGroup> iterator = pContainer.GetItemQueryIterator<ProductGroup>(qDef, continuationToken);
            while(iterator.HasMoreResults)
            {
                FeedResponse<ProductGroup> fResponse = await iterator.ReadNextAsync();
                foreach(var item in fResponse)
                {
                    Console.WriteLine($"{item.Name}. Nr of products: {item.Products.Count()}");
                }
            }
            Console.WriteLine("====================================================");
            query = @"SELECT root.Name AS GroupName, p.Brand, p.Name AS ProductName 
                            FROM root
                            JOIN p IN root.Products
                            WHERE p.Brand = 'Sony'";
            qDef = new QueryDefinition(query);
            FeedIterator<dynamic> pIterator = pContainer.GetItemQueryIterator<dynamic>(qDef);
            while (pIterator.HasMoreResults)
            {
                FeedResponse<dynamic> pResponse = await pIterator.ReadNextAsync();
                foreach (var item in pResponse)
                {
                    Console.WriteLine($"{item.GroupName}: {item.Brand} {item.ProductName}");
                }
            }
            Console.WriteLine("====================================================");
            var linq = pContainer.GetItemLinqQueryable<ProductGroup>();
            var fi = linq.Where(g => g.ID == "1").ToFeedIterator<ProductGroup>();
            while (fi.HasMoreResults)
            {
                var fResponse = await fi.ReadNextAsync();
                foreach (var item in fResponse)
                {
                    Console.WriteLine($"({item.ID}) {item.Name}");
                }
            }
        }
        private static async Task AddProductGroups(CosmosClient client)
        {
            var pContainer = client.GetContainer(Database, Container);
            var groups = await CreateGroupsAsync();
            foreach(var group in groups)
            {
                try
                {
                    var response = await pContainer.CreateItemAsync(group, new PartitionKey(group.PKey));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
        private static async Task<List<ProductGroup>> CreateGroupsAsync()
        {
            var groups = new List<ProductGroup>();
            var bRepo = new BrandRepository();
            var pRepo = new ProductRepository();
            var gRepo = new ProductGroupRepository();

            foreach (var pg in await gRepo.GetAllAsync(0, 1000))
            {
                var prods = await gRepo.GetProductsAsync(pg.ID);
                var npg = new CoreSqlTest.ProductGroup
                {
                    ID = pg.ID.ToString(),
                    Name = pg.Name,
                    PKey = pg.Name.FirstOrDefault().ToString(),
                    Products = prods.Select(p => new CoreSqlTest.Product
                    {
                        Name = p.Name,
                        Brand = p.Brand.Name,
                        BrandWebSite = p.Brand.Website
                    }).ToArray()
                };
                groups.Add(npg);
            }
            return groups;
        }
    }
}
