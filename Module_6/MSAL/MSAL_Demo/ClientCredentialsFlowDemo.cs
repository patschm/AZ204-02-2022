using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSAL_Demo
{
    public class ClientCredentialsFlowDemo : DemoBase
    {
        public override async Task DemoWithMSALAsync()
        {
            var app = ConfidentialClientApplicationBuilder.Create(ClientID)
                 .WithTenantId(TenantID)
                 .WithClientSecret(ClientSecret)
                 .Build();
            var token = await app
                .AcquireTokenForClient(new string[] { $"{ServiceToBeCalled}.default" })
                .ExecuteAsync();
            if (!string.IsNullOrEmpty(token.AccessToken))
            {
                await CallGraphApi(token.AccessToken, "users");
            }
        }
        public override async Task DemoWithoutMSALAsync()
        {
            // Make sure you have some Application Permissions set in App Registration e.g. User.Read.All
            // Services cannot use delegating permissions
            // 

            // Consent if you haven't done so.
            //var result = await GetPermissions();
            //if (result == null || result != "_consent=True")
            //{
            //    Console.WriteLine("No permission");
            //    return;
            //}
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token.Token))
            {
                await CallGraphApi(token.Token, "users");
            }
        }

        private async Task<string> GetPermissions()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"client_id={ClientID}");
            queryBuilder.Append($"&redirect-uri={RedirectUrl}");
            queryBuilder.Append($"&state=12345");

            BrowserLauncher browser = new BrowserLauncher();
            var req = new Uri($"https://login.microsoftonline.com/{TenantID}/adminconsent?{queryBuilder}");
            var redirUri = new Uri(RedirectUrl);
            var consent = await browser.StartBrowserPlatformAsync(req, redirUri);
            return consent.GetToken(redirUri);
        }

        private async Task<ResponseToken> GetTokenAsync()
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"client_id={ClientID}");
            bodyBuilder.Append($"&scope={ServiceToBeCalled}.default");
            bodyBuilder.Append($"&client_secret={ClientSecret}");
            bodyBuilder.Append($"&grant_type=client_credentials");

            var content = new StringContent(bodyBuilder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await Client.PostAsync($"{TenantUrl}token", content);
            return await ExtractTokenAsync(response);
        }
    }
}
