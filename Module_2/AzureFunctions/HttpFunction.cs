using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions
{
    public static class HttpFunction
    {
        [FunctionName("HttpFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{data}")] HttpRequest req,
            string data,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic dName = JsonConvert.DeserializeObject(requestBody);
            name = name ?? dName?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? $"Hello {data} Pass a name in the query string or in the request body for a personalized response."
                : $"Hello {data}, Your name is {name}.";

            return new OkObjectResult(responseMessage);
        }
    }
}
