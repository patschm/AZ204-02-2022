using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.IO;
using System.Reflection;
using System;
using System.Globalization;

namespace ConsoleClient
{
    public class SampleConfiguration
    {
        public PublicClientApplicationOptions PublicClientApplicationOptions { get; set; }

        public string MicrosoftGraphBaseEndpoint { get; set; }

        public static SampleConfiguration ReadFromJsonFile(string path)
        {
            // .NET configuration
            IConfigurationRoot Configuration;

            var builder = new ConfigurationBuilder()
             .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
            .AddJsonFile(path);

            Configuration = builder.Build();

            // Read the auth and graph endpoint config
            SampleConfiguration config = new SampleConfiguration()
            {
                PublicClientApplicationOptions = new PublicClientApplicationOptions()
            };
            Configuration.Bind("Authentication", config.PublicClientApplicationOptions);
            config.MicrosoftGraphBaseEndpoint = Configuration.GetValue<string>("WebAPI:MicrosoftGraphBaseEndpoint");
            return config;
        }
    }

}