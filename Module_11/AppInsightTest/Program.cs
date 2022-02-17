using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace AppInsightTest
{
    class Program
    {
        static TelemetryConfiguration configuration;
        static TelemetryClient telemetryClient;
        static ILogger logger;
        static string logFolder = @"E:\Logs";

        static void Main(string[] args)
        {
            configuration = TelemetryConfiguration.CreateDefault();
            configuration.ConnectionString = "InstrumentationKey=4a0d620e-a15e-44b6-b190-ba6d7fa944e2;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/";
            TeleChannels(configuration, local:false);


            telemetryClient = new TelemetryClient(configuration);
            telemetryClient.Context.Operation.Name = "Main";
            telemetryClient.Context.Session.Id = "Me Console";
            telemetryClient.Context.User.AccountId = "Me";
            telemetryClient.Context.User.Id = Guid.NewGuid().ToString();
            telemetryClient.Context.Cloud.RoleName = "Console";
            telemetryClient.Context.Device.OperatingSystem = "Windows 12";
            
            // For live Metrics
            LiveMetricsSingUp(configuration);
            
            // Special ILogger
            logger = new ApplicationInsightsLogger("Console", telemetryClient, new ApplicationInsightsLoggerOptions { });
            WriteData();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
            telemetryClient.Flush();
        }

        private static void TeleChannels(TelemetryConfiguration configuration, bool local = false, bool devMode = true)
        {
            if (local)
            {
                _27McChannel channel = new _27McChannel {
                    FileName = @"E:\logging.log"
                };
                configuration.TelemetryChannel = channel; 
                //Console.WriteLine($"Writing to: {channel.FileName}");
            }
            else
            {
                var channel = new ServerTelemetryChannel { 
                    DeveloperMode = devMode 
                };
                channel.Initialize(configuration);
               
            }
        }

        private static void LiveMetricsSingUp(TelemetryConfiguration configuration)
        {
            QuickPulseTelemetryProcessor processor = null;
            configuration.TelemetryProcessorChainBuilder.Use((next) => {
                processor = new QuickPulseTelemetryProcessor(next);
                return processor;
            }).Build();

            var QuickPulse = new QuickPulseTelemetryModule();
            QuickPulse.Initialize(configuration);
            QuickPulse.RegisterTelemetryProcessor(processor);
        }

        private static void WriteData()
        {      
            ConsoleKey key;
            int counter = 0;
            do
            {
                Console.WriteLine("New Trace? (Press e for an error)");
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.E)
                {
                    telemetryClient.TrackException(new Exception("Oooops"));
                }
                logger.LogInformation("Info");
                logger.LogCritical("Critical");
                logger.LogError("Error");
                telemetryClient.TrackEvent($"Hello World {++counter}", new Dictionary<string, string> { { "Count", $"{counter}" } });
                telemetryClient.TrackTrace($"Hello World {++counter}", new Dictionary<string, string>{ { "Count", $"{counter}" } });
            } 
            while (key != ConsoleKey.Escape);
            telemetryClient.Flush();
        }
    }
}
