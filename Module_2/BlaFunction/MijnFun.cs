using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace BlaFunction
{
    public static class MijnFun
    {
        [FunctionName("TimerFunction")]
        public static void Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, 
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }

        [FunctionName("TrekkerFunction")]
        public static IActionResult Loop(
            [HttpTrigger(AuthorizationLevel.Anonymous, new string[]{ "get" }, Route = "bang/{name}")]
            HttpRequest context,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            return new OkObjectResult($"<h1>{name}</h1>");
        }

    }
}
