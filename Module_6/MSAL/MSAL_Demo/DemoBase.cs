using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSAL_Demo
{
    public abstract class DemoBase
    {
        public static string TenantID = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
        public static string ClientID = "0c5d75ca-66aa-479b-b39b-2b3ad6f93831";
        public static string ClientSecret = "syg7Q~Fn3sknzb.ZrRxAqT~OSMRjVY2mzLIhl";
        public static string[] Scopes = new string[] { "user.read", "profile", "openid", "email" };
        public static string BaseRedirectUrl = "http://localhost:8080";
        public static string RedirectUrl = $"{BaseRedirectUrl}/console";
        public static string TenantUrl = $"https://login.microsoftonline.com/{TenantID}/oauth2/v2.0/";
        public static string CommonUrl = $"https://login.microsoftonline.com/common/oauth2/v2.0/";
        public static string OrganizationUrl = $"https://login.microsoftonline.com/organizations/oauth2/v2.0/";
        public static string ServiceToBeCalled = "https://graph.microsoft.com/";

        protected static HttpClient Client = new HttpClient();

        public abstract Task DemoWithoutMSALAsync();
        public abstract Task DemoWithMSALAsync();
        protected async Task<ResponseToken> ExtractTokenAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                JObject jresult = JsonConvert.DeserializeObject(res) as JObject;
                return new ResponseToken
                {
                    Token = jresult.Property("access_token").Value.ToString(),
                    RefreshToken = jresult.Property("refresh_token")?.Value.ToString()
                };
            }
            else
            {
                Console.WriteLine($"Response: {response.StatusCode}");
                var ct = await response.Content.ReadAsStringAsync();
                Console.WriteLine(ct);
            }
            return null;
        }
        protected async Task<string> ExtractDeviceTokenAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                JObject jresult = JsonConvert.DeserializeObject(res) as JObject;
                Console.WriteLine(jresult.Property("message").Value);
                return jresult.Property("device_code").Value.ToString();
            }
            else
            {
                Console.WriteLine($"Response: {response.StatusCode}");
                var ct = await response.Content.ReadAsStringAsync();
                Console.WriteLine(ct);
            }
            return null;
        }
        protected async Task CallGraphApi(string accessToken, string path = "me")
        {
            var authHeader = new AuthenticationHeaderValue("bearer", accessToken);
    
            Client.DefaultRequestHeaders.Authorization = authHeader;
            HttpResponseMessage response = await Client.GetAsync($"{ServiceToBeCalled}v1.0/{path}");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==============================================================");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jresult = JsonConvert.DeserializeObject(json) as JObject;
                foreach (JProperty child in jresult.Properties())
                {
                    Console.WriteLine($"{child.Name} = {child.Value}");
                }
            }
            Console.ResetColor();
        }
    }
}
