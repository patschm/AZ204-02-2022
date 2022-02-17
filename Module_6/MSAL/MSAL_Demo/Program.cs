using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MSAL_Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Make sure you consented these Graph Permissions in Azure AD
            DemoBase.Scopes = new string[] { "user.read", "profile", "openid", "email", "offline_access" }; // offline_access for refresh token
            DemoBase.TenantID = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
            DemoBase.ClientID = "0c5d75ca-66aa-479b-b39b-2b3ad6f93831";
            DemoBase.ClientSecret = "syg7Q~Fn3sknzb.ZrRxAqT~OSMRjVY2mzLIhl";

            // AAD Endpoints
            DemoBase.TenantUrl = $"https://login.microsoftonline.com/{DemoBase.TenantID}/oauth2/v2.0/";
            DemoBase.CommonUrl = $"https://login.microsoftonline.com/common/oauth2/v2.0/";
            DemoBase.OrganizationUrl = $"https://login.microsoftonline.com/organizations/oauth2/v2.0/";

            // Register the following ReturnUrls in your Application Registration(See Registrations.jpg)
            DemoBase.BaseRedirectUrl = "http://localhost:8080";
            DemoBase.RedirectUrl = $"{DemoBase.BaseRedirectUrl}/console";
           

            DemoBase demo;
            //demo = new PasswordFlowDemo();
            demo = new CodeFlowDemo();
            //demo = new ImplicitFlowDemo();
            //demo = new ClientCredentialsFlowDemo();
            //demo = new DeviceCodeFlowDemo();

            await demo.DemoWithoutMSALAsync();
            //await demo.DemoWithMSALAsync();
        
            Console.WriteLine("Done! Enter to continue");
            Console.ReadLine();
        }

        //private static async Task CodeFlowAsync()
        //{
        //    var authz = await GetAuthZWithoutMSALAsync();
        //    var token = await GetTokenWithoutMSALAsync(authz);
        //    await CallGraphApi(token);

        //    async Task<string> GetAuthZWithoutMSALAsync()
        //    {
        //        var queryBuilder = new StringBuilder();
        //        queryBuilder.Append($"client_id={clientID}");
        //        queryBuilder.Append($"&response_type=code"); // authz code
        //        queryBuilder.Append($"&response_mode=query");
        //        queryBuilder.Append($"&redirect-uri={redirectUrl}");
        //        queryBuilder.Append($"&scope={string.Join(' ', scopes)}");
        //        queryBuilder.Append($"&state=12345");

        //        BrowserLauncher browser = new BrowserLauncher();
        //        var req = new Uri($"{tenantUrl}authorize?{queryBuilder}");
        //        var redirUri = new Uri(redirectUrl);
        //        var authz = await browser.StartBrowserPlatformAsync(req, redirUri);
        //        return authz.GetAuthZToken(redirUri);
        //    }
        //    async Task<string> GetTokenWithoutMSALAsync(string authz)
        //    { 
        //        var bodyBuilder = new StringBuilder();
        //        bodyBuilder.Append($"client_id={clientID}");
        //        bodyBuilder.Append($"&scope={string.Join(' ', scopes)}");
        //        bodyBuilder.Append($"&code={authz}");
        //        bodyBuilder.Append($"&grant_type=authorization_code");

        //        var content = new StringContent(bodyBuilder.ToString());
        //        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        //        HttpClient client = new HttpClient { BaseAddress = new Uri(tenantUrl) };
        //        var response = await client.PostAsync("token", content);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            string res = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(res);
        //            JObject jresult = JsonConvert.DeserializeObject(res) as JObject;
        //            return jresult.Property("access_token").Value.ToString();
        //        }
        //        else
        //        {
        //            Console.WriteLine($"Response: {response.StatusCode}");
        //            var ct = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(ct);
        //        }
        //        return null;
        //    }
        //}

        //private static async Task ImplicitFlowAsync()
        //{
        //    // See ImplicitFlow project
        //}
        
        
    }
}
