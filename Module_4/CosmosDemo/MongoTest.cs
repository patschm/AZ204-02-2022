using Entities;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Security;
using Repository.InMemory;
using Gremlin.Net.Process.Traversal;

namespace CosmosDemo
{
    static class MongoTest
    {
        public static string Host = "ps-mongo.mongo.cosmos.azure.com";
        public static int Port = 10255;
        public static string DBase = "productsDB";
        public static string User = "ps-mongo";
        public static string Password = "65jUA5dZnAFets50eEcW9dDOZM6S416dY0xtBFdJ52oLIamfWfJdRmbxcd3sRmcwUvIGXBcFODfjgFTgbaV6ew==";
        public static string Collection = "products";
        private static string testID = "4032";

        public static async Task RunMongo()
        {
            IMongoClient client = await CreateClientAsync();
            IMongoDatabase table = await GetDatabaseAsync(client);
            IMongoCollection<ProductDocument> container = await GetContainerAsync(table);
            //await WriteDataAsync(container);
            await ReadDataAsync(container);
            //await UpdateDataAsync(container);
            //await DeleteDataAsync(container);
            Console.WriteLine("Done!");
        }
        private static Task<MongoClient> CreateClientAsync()
        {
            BsonClassMap.RegisterClassMap<ProductDocument>(m =>
            {
                m.AutoMap();
                m.SetIgnoreExtraElements(true);
            });

            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(Host, Port);
            settings.UseTls = true;
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
            settings.RetryWrites = false;
            MongoIdentity iden = new MongoInternalIdentity(DBase, User);
            MongoIdentityEvidence evidence = new PasswordEvidence(Password);
            settings.Credential = new MongoCredential("SCRAM-SHA-1", iden, evidence);

            MongoClient client = new MongoClient(settings);

            return Task.FromResult(client);
        }

        private static Task<IMongoCollection<ProductDocument>> GetContainerAsync(IMongoDatabase database)
        {
            IMongoCollection<ProductDocument> coll = database.GetCollection<ProductDocument>(Collection);
            return Task.FromResult(coll);
        }
        private static Task<IMongoDatabase> GetDatabaseAsync(IMongoClient client)
        {
            var db = client.GetDatabase(DBase);
            return Task.FromResult(db);
        }

        private static async Task DeleteDataAsync(IMongoCollection<ProductDocument> collection)
        {
            var flt = Builders<ProductDocument>.Filter.Eq(f => f.ID, testID);
            await collection.DeleteManyAsync(flt);

            var q = from rec in collection.AsQueryable() where rec.ID == testID select rec;
            var pr = await q.FirstOrDefaultAsync();
        }
        private static async Task UpdateDataAsync(IMongoCollection<ProductDocument> collection)
        {
            var q = from rec in collection.AsQueryable() where rec.ID == testID select rec;
            var pr = await q.FirstOrDefaultAsync();
            pr.Name = "Sandra Jones";
            var flt = Builders<ProductDocument>.Filter.Eq(f => f.ID, testID);
            await collection.ReplaceOneAsync(flt, pr);
            pr = await q.FirstOrDefaultAsync();
            Console.WriteLine(pr);
        }
        private static async Task ReadDataAsync(IMongoCollection<ProductDocument> collection)
        {
            // Basic
            FilterDefinition<ProductDocument> filter = Builders<ProductDocument>.Filter.Eq(p => p.ID, testID);
            IAsyncCursor<ProductDocument> cursor = await collection.FindAsync(filter);
            ProductDocument pr = await cursor.FirstOrDefaultAsync();
            Console.WriteLine(pr);

            // Via Linq
            var q = from rec in collection.AsQueryable() where rec.ID == testID select rec;
            pr = await q.FirstOrDefaultAsync();
            Console.WriteLine(pr);
        }
        private static async Task WriteDataAsync(IMongoCollection<ProductDocument> collection)
        {
            var bRepo = new BrandRepository();
            var pRepo = new ProductRepository();
            var gRepo = new ProductGroupRepository();
            foreach (var product in await pRepo.GetAllAsync(0, 100))
            {
                ProductDocument doc = new ProductDocument
                {
                    ID = product.ID.ToString(),
                    Name = product.Name,
                    Brand = new ProductDocument.BrandDocument
                    {
                        ID = product.Brand?.ID.ToString(),
                        Name = product.Brand?.Name
                    },
                    ProductGroups = (await gRepo.GetProductGroupsAsync(product.ID))
                    .Select(pg => new ProductDocument.ProductGroupDocument
                    {
                        ID = pg.ID.ToString(),
                        Name = pg.Name
                    }).ToList()
                };
                await collection.InsertOneAsync(doc);
            }
        }

    }
}
