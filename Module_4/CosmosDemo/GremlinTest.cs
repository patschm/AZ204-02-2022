using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Gremlin.Net.Structure.IO.GraphSON;
using Entities;
using System.Reflection;
using Gremlin.Net.Process.Traversal;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.IO;
using Repository.InMemory;

namespace CosmosDemo
{
    static class GremlinTest
    {
        private static string Host = "ps-gremlin.gremlin.cosmosdb.azure.com";
        private static string PrimaryKey = "Jf8TYj4WyRSaGKaaHWkC5pEeW5X5MVz8CaN81xnqedLj17ZZcSiCZ8a2Ra14dhGbdqVWkWzBL84ykOzjm3VLFA==";
        private static string Database = "graphdb";
        private static string Container = "products";
        public static int Port = 443;
        public static bool EnableSSL = true;
        public static string testID = "1070";

        public static async Task RunGremlin()
        {
            var client = CreateGremlinClient();
            //await AddProducts(client);
            await ReadData(client);
            Console.WriteLine("Done");

        }

        private static GremlinClient CreateGremlinClient()
        {
            string containerLink = "/dbs/" + Database + "/colls/" + Container;
            var gremlinServer = new GremlinServer(Host, Port, enableSsl: EnableSSL, username: containerLink, password: PrimaryKey);
            //return new GremlinClient(gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType);
            return new GremlinClient(gremlinServer, new GraphSON2MessageSerializer());
        }
        private static async Task ReadData(GremlinClient client)
        {
            var result = await client.SubmitAsync<dynamic>($@"g.V('{testID}').hasLabel('product')");//.out('brand')");
            var first = result.First();
            var res = Newtonsoft.Json.JsonConvert.SerializeObject(result.First());
            Console.WriteLine(res);
        }
        private static async Task AddProducts(GremlinClient client)
        {
            var bRepo = new BrandRepository();
            var pRepo = new ProductRepository();
            var gRepo = new ProductGroupRepository();

            foreach (var brand in await bRepo.GetAllAsync(0, 1000))
            {
                await AddBrand(client, brand);
            }
            foreach (var group in await gRepo.GetAllAsync(0, 10000))
            {
                await AddProductGroup(client, group);
            }
            foreach (var prod in await pRepo.GetAllAsync(0, 10000))
            {
                await AddProduct(client, prod);
            }

        }

        private static async Task AddProductGroup(GremlinClient client, ProductGroup group)
        {
            var gRepo = new ProductGroupRepository();
            try
            {
                string name = group.Name.Replace("\'", "");
                string query = $@"g.AddV('productgroup')
                                                    .property('id', '{group.ID}')
                                                    .property('name', '{name}')
                                                    .property('pkey', '{name.ToLower().First()}')";
                var result = await client.SubmitAsync<dynamic>(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static async Task AddBrand(GremlinClient client, Brand brand)
        {
            try
            {
                var result = await client.SubmitAsync<dynamic>($@"g.addV('brand')
                                                    .property('id', '{brand.ID}')
                                                    .property('name', '{brand.Name}')
                                                    .property('website', '{brand.Website}')
                                                    .property('pkey', '{brand.Name.ToLower().First()}')");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        private static async Task AddProduct(GremlinClient client, Product p)
        {
            try
            {
                var gRepo = new ProductGroupRepository();
                string name = p.Name.Replace("'", "");
                var result = await client.SubmitAsync<dynamic>($@"g.AddV('product')
                                                    .property('id', '{p.ID}')
                                                    .property('name', '{name}')
                                                    .property('pkey', '{name.ToLower().First()}')");

                result = await client.SubmitAsync<dynamic>($@"g.V('{p.ID}').addE('brand').to(g.V('{p.BrandID}'))");

                foreach (var group in await gRepo.GetProductGroupsAsync(p.ID))
                {
                    result = await client.SubmitAsync<dynamic>($@"g.V('{p.ID}').addE('groups').to(g.V('{group.ID}'))");
                    result = await client.SubmitAsync<dynamic>($@"g.V('{group.ID}').addE('products').to(g.V('{p.ID}'))");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
