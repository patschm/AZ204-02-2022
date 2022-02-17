using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Durability
{
    public static class Orchestrator
    {
        // Triggers the Orchestrator
        [FunctionName("Orchestrator_HttpStart")]
        public static async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{scenario}")] HttpRequestMessage req,
            string scenario,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Starts the Orchestrator with unique identifier that will be used to check states and such
            string instanceId = "";
            
            switch(scenario.ToLower())
            {
                case "a":
                    {
                        instanceId = await starter.StartNewAsync("Pipe_Orchestrator", "unique_identifier");
                        break;
                    }
                case "b":
                    {
                        instanceId = await starter.StartNewAsync("Fan_Orchestrator", "unique_identifier");
                        break;
                    }
                default:
                    {
                        instanceId = await starter.StartNewAsync("Pipe_Orchestrator", "unique_identifier");
                        break;
                    }
            }
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            var bld = new StringBuilder();
            var info =  starter.CreateCheckStatusResponse(req, instanceId);
            dynamic data = JsonConvert.DeserializeObject<dynamic>(await info.Content.ReadAsStringAsync());
            
            bld.Append("<html><head></head><body><table>");
            bld.Append($"<h1>Info for instance {instanceId}</h1>");
            bld.Append($"<tr><td><h2>statusQueryGetUri:</h2></td><td><h2>{data.statusQueryGetUri}</h2></td></tr>");
            bld.Append($"<tr><td><h2>sendEventPostUri:</h2></td><td><h2>{data.sendEventPostUri}</h2></td></tr>");
            bld.Append($"<tr><td><h2>terminatePostUri:</h2></td><td><h2>{data.terminatePostUri}</h2></td></tr>");
            bld.Append("</table></body></html>");
            return new ContentResult { ContentType = "text/html", Content = bld.ToString() };
        }

        [FunctionName("Pipe_Orchestrator")]
        public static async Task<string> RunOrchestratorPipe(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            // Replace "hello" with the name of your Durable Activity Function.
            var res = await context.CallActivityAsync<string>("Orchestrator_Hello", "Tokyo");
            log.LogError($"Pipe Says: [{res}]");
            res = await context.CallActivityAsync<string>("Orchestrator_Hello", "Seattle");
            log.LogError($"Pipe Says: [{res}]");
            res = await context.CallActivityAsync<string>("Orchestrator_Hello", "London");
            log.LogError($"Pipe Says: [{res}]");

            return "OK";
        }
        [FunctionName("Fan_Orchestrator")]
        public static async Task<string> RunOrchestratorFan(
            [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var outputs = new List<Task<string>>();
            outputs.Add(context.CallActivityAsync<string>("Orchestrator_Hello", "Tokyo"));
            outputs.Add(context.CallActivityAsync<string>("Orchestrator_Hello", "Seattle"));
            outputs.Add(context.CallActivityAsync<string>("Orchestrator_Hello", "London"));

            await Task.WhenAll(outputs);
            var result = "";
            outputs.ForEach(item => result += $"[{item.Result}], " );
            log.LogError($"Fan Says: {result}");
            return result;
        }

        [FunctionName("Orchestrator_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogError($"Saying hello to {name}.");
            return $"Hello {name}!";
        }
    }
}