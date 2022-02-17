using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AppInsightTest
{
    public class _27McChannel : ITelemetryChannel
    {
        private StreamWriter _logFile;

        public bool? DeveloperMode { get; set; } = false;
        public string EndpointAddress { get; set; } = null;
        public string FileName { 
            set 
            {
                try
                {
                    _logFile = new StreamWriter(value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            } 
        }


        public void Dispose()
        {
            _logFile?.Dispose();
        }

        public void Flush()
        {
            _logFile.Flush();
        }

        public void Send(ITelemetry item)
        {
            _logFile.WriteLine($@"{item.Context.Session.Id} {item.Context.User.Id} {item.Context.Cloud.RoleName}: {item.Context.Operation.Name}");
        }
    }
}
