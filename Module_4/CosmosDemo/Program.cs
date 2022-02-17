using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Entities;
using System.Collections.Generic;
using Newtonsoft.Json;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Gremlin.Net.Driver.Exceptions;

namespace CosmosDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await MongoTest.RunMongo();
            //await GremlinTest.RunGremlin();
            //await CoreSqlTest.RunCoreSql();
            await ChangeFeedTest.RunCoreSql();
            Console.WriteLine("Done");
            Console.ReadLine();
        }

    }
}
