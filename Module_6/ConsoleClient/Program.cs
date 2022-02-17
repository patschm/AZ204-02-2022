using Entities;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        // Register in AAD
        // Modify Manifest.
        // In the manifest editor, set the allowPublicClient property to true
        private static string clientId = "247e01d0-f4ff-40f4-92c0-5b749e272ac1";
        private static string tenantID = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";

        static async Task Main(string[] args)
        {
            //await Basic();
            await CallService();
            Console.ReadLine();
        }
        private static async Task Basic()
        {
            string[] scopes = { "User.Read", "User.ReadBasic.All" };

            IPublicClientApplication app = PublicClientApplicationBuilder.Create(clientId)
                //.WithAuthority(AadAuthorityAudience.AzureAdMultipleOrgs)
                .WithAuthority(AadAuthorityAudience.AzureAdMyOrg)
                .WithTenantId(tenantID)
                .WithRedirectUri("http://localhost")
                .Build();

            var result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

            Console.WriteLine(result.AccessToken);
            Console.WriteLine($"Hello {result.Account.Username}");


            HttpClient client = new HttpClient();
            var defaultRequestHeaders = client.DefaultRequestHeaders;
            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);

            Console.WriteLine("=========================================================");
            HttpResponseMessage response = await client.GetAsync("https://graph.microsoft.com/v1.0/me");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jresult = JsonConvert.DeserializeObject(json) as JObject;
                foreach (JProperty child in jresult.Properties().Where(p => !p.Name.StartsWith("@")))
                {
                    Console.WriteLine($"{child.Name} = {child.Value}");
                }
            }
            Console.WriteLine("=========================================================");
            response = await client.GetAsync("https://graph.microsoft.com/v1.0/users");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jresult = JsonConvert.DeserializeObject(json) as JObject;
                foreach (JProperty child in jresult.Properties().Where(p => !p.Name.StartsWith("@")))
                {
                    Console.WriteLine($"{child.Name} = {child.Value}");
                }
            }
        }
        private static async Task CallService()
        {
            string[] scopes = { "api://96d8bee8-7f67-48e4-a338-a0b59d0419b9/Readers" };

            string uri = $"https://login.microsoftonline.com/{tenantID}/v2.0";
            IPublicClientApplication app = PublicClientApplicationBuilder.Create(clientId)
                .WithAuthority(AadAuthorityAudience.AzureAdMyOrg)
                .WithTenantId(tenantID)
                .WithRedirectUri("http://localhost")
                .Build();

            var result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
            Console.WriteLine(result.AccessToken);

            HttpClient client = new HttpClient();
            var defaultRequestHeaders = client.DefaultRequestHeaders;
            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.AccessToken);

            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/brands");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var jresult = JsonConvert.DeserializeObject<List<Brand>>(json);
                foreach (var brand in jresult)
                {
                    Console.WriteLine($"{brand.Name}, ({brand.Website})");
                }
            }
        }
    }
}
