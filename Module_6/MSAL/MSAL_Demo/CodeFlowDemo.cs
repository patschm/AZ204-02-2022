using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSAL_Demo
{
    public class CodeFlowDemo: DemoBase
    {
        public override async Task DemoWithMSALAsync()
        {
            var app = PublicClientApplicationBuilder.Create(ClientID)
                .WithTenantId(TenantID)
                .WithRedirectUri(BaseRedirectUrl) 
                .Build();

           // app.UserTokenCache.SetAfterAccess(TokenCacheSpy);
            // Returns you:
            // - Access Token
            // - Id Token
            // - Refresh Token
            // All will be stored in app.UserTokenCache
             var token = await app.AcquireTokenInteractive(Scopes).ExecuteAsync();
            
            await CallGraphApi(token.AccessToken);

            // Refreshes token if needed
            var accounts = await app.GetAccountsAsync();
            token = await app.AcquireTokenSilent(Scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();
            await CallGraphApi(token.AccessToken);
        }
        public override async Task DemoWithoutMSALAsync()
        {
            var authz = await GetAuthZTokenAsync();
            var token = await GetTokenAsync(authz);
            if (!string.IsNullOrEmpty(token.Token))
            {
                await CallGraphApi(token.Token);
            }
            token = await RefreshTokenAsync(token.RefreshToken);
            if (!string.IsNullOrEmpty(token.Token))
            {
                await CallGraphApi(token.Token);
            }
        }

        #region Without MSAL   
        private async Task<string> GetAuthZTokenAsync()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append($"client_id={ClientID}");
            queryBuilder.Append($"&response_type=code"); // authz code
            queryBuilder.Append($"&response_mode=query");
            queryBuilder.Append($"&redirect_uri={RedirectUrl}");
            queryBuilder.Append($"&scope={string.Join(' ', Scopes)}");
            queryBuilder.Append($"&state=12345");

            BrowserLauncher browser = new BrowserLauncher();
            var req = new Uri($"{TenantUrl}authorize?{queryBuilder}");
            var redirUri = new Uri(RedirectUrl);
            var authz = await browser.StartBrowserPlatformAsync(req, redirUri);
            return authz.GetToken(redirUri);
        }
        private async Task<ResponseToken> GetTokenAsync(string authz)
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"client_id={ClientID}");
            bodyBuilder.Append($"&client_secret={ClientSecret}");
            bodyBuilder.Append($"&scope={string.Join(' ', Scopes)}");
            bodyBuilder.Append($"&redirect_uri={RedirectUrl}");
            bodyBuilder.Append($"&code={authz}");
            bodyBuilder.Append($"&grant_type=authorization_code");
            // If you want to do code flow with PKCE:
            //bodyBuilder.Append($"code_verifier = 7073d688b6dcb02b9a2332e0792be265b9168fda7a6");

              var content = new StringContent(bodyBuilder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await Client.PostAsync($"{TenantUrl}token", content);
            return await ExtractTokenAsync(response);
        }
        private async Task<ResponseToken> RefreshTokenAsync(string refreshToken)
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"client_id={ClientID}");
            bodyBuilder.Append($"&client_secret={ClientSecret}");
            bodyBuilder.Append($"&scope={string.Join(' ', Scopes)}");
            bodyBuilder.Append($"&refresh_token={refreshToken}");
            bodyBuilder.Append($"&grant_type=refresh_token");

            var content = new StringContent(bodyBuilder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await Client.PostAsync($"{TenantUrl}token", content);
            return await ExtractTokenAsync(response);
        }
        #endregion
        #region With MSAL
        private void TokenCacheSpy(TokenCacheNotificationArgs tcna)
        {
            byte[] data = tcna.TokenCache.SerializeMsalV3();
            var json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data)) as JObject;
            Console.WriteLine($"Access Token: {json.SelectToken("AccessToken")}");
            Console.WriteLine($"Refresh Token: {json.SelectToken("RefreshToken")?.ToString()}");
            Console.WriteLine($"ID Token: {json.SelectToken("IdToken")?.ToString()}");
        }
        #endregion
    }
}
