using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging.AzureAppServices;

namespace WebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.Configure<AzureBlobLoggerOptions>(options =>
                    {
                        options.BlobName = "webclient_log.txt";
                    });
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddApplicationInsights(context.Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(config =>
                    {
                        var settings = config.Build();
                        // dotnet user-secrets set ConnectionStrings:AppConfig "<your_connection_string>"
                        //var connection = settings.GetConnectionString("AppConfig");
                        var connection = "Endpoint=https://ps-paroducts-config.azconfig.io;Id=Y50E-l9-s0:z7Og42GLdBh2wU1SKCLm;Secret=WYhCdZGsgYdeKb9uD/5YE7e7aRpq0W6i/d2JeOe2o8o=";
                        config.AddAzureAppConfiguration(connection);
                    });
                    webBuilder.UseStartup<Startup>();
                });
        /*
                .ConfigureLogging((context, builder) =>
                 {
                    // Providing an instrumentation key is required if you're using the
                    // standalone Microsoft.Extensions.Logging.ApplicationInsights package,
                    // or when you need to capture logs during application startup, such as
                    // in Program.cs or Startup.cs itself.
                    builder.AddApplicationInsights(context.Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

                    // Capture all log-level entries from Program
                    builder.AddFilter<ApplicationInsightsLoggerProvider>(
                        typeof(Program).FullName, LogLevel.Trace);

                    // Capture all log-level entries from Startup
                    builder.AddFilter<ApplicationInsightsLoggerProvider>(
                        typeof(Startup).FullName, LogLevel.Trace);
                });
        */
    }
}
