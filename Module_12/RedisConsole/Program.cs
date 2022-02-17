using StackExchange.Redis;
using System;
using System.Data;
using System.Reflection.Metadata;

namespace RedisConsole
{
    class Program
    {
        private static string conStr = "ps-cash.redis.cache.windows.net:6380,password=6KKJmOkPW0mfOiKibHrE5pbV3F1OuOg8eAzCaGRwCLs=,ssl=True,abortConnect=False";
        private static ConnectionMultiplexer connection;

        static void Main(string[] args)
        {
            connection = ConnectionMultiplexer.Connect(conStr);

            Primitive();
        
            connection.Dispose();
            Console.WriteLine("Done");
            Console.ReadLine();
                       
        }
        private static void Primitive()
        {
            IDatabase cache = connection.GetDatabase();

            //cache.
            cache.StringSet("Testje", "Hallo", TimeSpan.FromSeconds(60), When.Exists);

            string cacheCommand = "PING";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : " + cache.Execute(cacheCommand).ToString());

            cacheCommand = "GET Message";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet("Message").ToString());

            cacheCommand = "SET Message \"Hello! The cache is working from a .NET Core console app!\"";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringSet()");
            Console.WriteLine("Cache response : " + cache.StringSet("Message", "Hello! The cache is working from a .NET Core console app!").ToString());

            cacheCommand = "GET Message";
            Console.WriteLine("\nCache command  : " + cacheCommand + " or StringGet()");
            Console.WriteLine("Cache response : " + cache.StringGet("Message").ToString());

            cacheCommand = "CLIENT LIST";
            Console.WriteLine("\nCache command  : " + cacheCommand);
            Console.WriteLine("Cache response : \n" + cache.Execute("CLIENT", "LIST").ToString().Replace("id=", "id="));
        }
    }
}
