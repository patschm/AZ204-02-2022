
using System;
using System.Threading.Tasks;

#region KeyVault
using Microsoft.IdentityModel.Clients.ActiveDirectory;
#endregion

#region AppConfiguration
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
#endregion


namespace KeyVault
{
    class Program
    {
        static string tenentId = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
        static string clientId = "cc1a6d20-23fe-4803-8046-f6c02e3bf61f";
        static string clientSecret = "DqX7Q~FxjZhn27LoPSIv4krNpvKGQ1yD5i0wv";
        static string kvUri = "https://ps-sleutelbossen.vault.azure.net/";
        
        static async Task Main(string[] args)
        {
           // await ReadKeyVault();
            await ReadAppConfigurationAsync();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        private static async Task ReadKeyVault()
        {
            ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
            SecretClient kvClient = new SecretClient(new Uri(kvUri), cred);
                
            var result = await kvClient.GetSecretAsync("MijnGeheim");
            Console.WriteLine($"Hello {result.Value?.Value}");
        }

        private static async Task ReadAppConfigurationAsync()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json")
                   .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

           

            //ReadLocal();
            await ReadRemoteAsync();

            void ReadLocal()
            {
                Console.WriteLine(configuration["MySetings:hello"]);
                Console.WriteLine(configuration["ConnectionString"]);
            }

            async Task ReadRemoteAsync()
            {
                builder.AddAzureAppConfiguration(opts => {
                    opts.ConfigureKeyVault(kvopts =>
                    {
                        kvopts.SetCredential(new ClientSecretCredential(tenentId, clientId, clientSecret));
                    });
                    opts.Connect(configuration["ConnectionString"]);    
                   
                });
                IConfiguration conf = builder.Build();

                Console.WriteLine($"{conf["Production:Connectionstring"]}");
                Console.WriteLine($"Hello {conf["Sleute"]}");

                IServiceCollection services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

                using (var svcProvider = services.BuildServiceProvider())
                {
                    var featureManager = svcProvider.GetRequiredService<IFeatureManager>();
                    if (await featureManager.IsEnabledAsync("test"))
                    {
                        Console.WriteLine("We have a new feature");
                    }
                }

            }
        }

    }
}
