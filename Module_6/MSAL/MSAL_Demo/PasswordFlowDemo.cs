using Microsoft.Identity.Client;
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
    public class PasswordFlowDemo: DemoBase
    {
        public override async Task DemoWithMSALAsync()
        {
            // Not recommended and will be removed from future OAuth specifications
            string username;
            string password;
            LoginScreen(out username, out password);
            
            // For password flow user _MUST_ be a domain user;
            username = "patrick@pschmidtpasit.onmicrosoft.com";

            var result = await PublicClientApplicationBuilder.Create(ClientID)
                                    .WithTenantId(TenantID)
                                    .Build()
                                    .AcquireTokenByUsernamePassword(Scopes, username, password.ToSecureString())
                                    .ExecuteAsync();
            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                await CallGraphApi(result.AccessToken);
            }
        }
        public override async Task DemoWithoutMSALAsync()
        {
            // Not recommended and will be removed from future OAuth specifications
            string username;
            string password;
            LoginScreen(out username, out password);
            // For password flow user _MUST_ be a domain user;
            username = "patrick@pschmidtpasit.onmicrosoft.com";
            var token = await GetTokenAsync(username, password);
            if (!string.IsNullOrEmpty(token.Token))
            {
                await CallGraphApi(token.Token);
            }
        }

        private async Task<ResponseToken> GetTokenAsync(string username, string password)
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"client_id={ClientID}");
            bodyBuilder.Append($"&client_secret={ClientSecret}");
            bodyBuilder.Append($"&scope={string.Join(' ', Scopes)}");
            bodyBuilder.Append($"&username={username}");
            bodyBuilder.Append($"&password={password}");
            bodyBuilder.Append($"&grant_type=password");

            var content = new StringContent(bodyBuilder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await Client.PostAsync($"{TenantUrl}token", content);
            return await ExtractTokenAsync(response);
        }
        private void LoginScreen(out string username, out string password)
        {
            Console.Write("Login: ");
            username = Console.ReadLine();
            Console.Write("Password: ");
            Console.ForegroundColor = Console.BackgroundColor;
            password = Console.ReadLine();
            Console.ResetColor();
        }
    }
}
