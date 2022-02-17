using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Duurzaam
{
    public static class Function2
    {
        [FunctionName("De_Orkest_Leider")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<Task<string>>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(context.CallActivityAsync<string>("Trompettist", "Tokyo"));
            outputs.Add(context.CallActivityAsync<string>("Trompettist", "Seattle"));
            outputs.Add(context.CallActivityAsync<string>("Trompettist", "London"));

            await Task.WhenAll(outputs);
            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            string res = "";
            outputs.ForEach(p => res += p.Result);
            return res;
        }

        [FunctionName("Trompettist")]
        public static async Task<string> SayHello([ActivityTrigger] string name, ILogger log)
        {
            await Task.Delay(30000);
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        [FunctionName("Geeft_OrkestLeider_een_trap")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("De_Orkest_Leider", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}