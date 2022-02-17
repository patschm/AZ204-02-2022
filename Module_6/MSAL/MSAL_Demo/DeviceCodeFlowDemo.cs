using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MSAL_Demo
{
    public class DeviceCodeFlowDemo: DemoBase
    {
        public override async Task DemoWithMSALAsync()
        {
            var app = PublicClientApplicationBuilder.Create(ClientID)
                .WithTenantId(TenantID)
                .Build();
            var token = await app.AcquireTokenWithDeviceCode(Scopes, cb =>
                {
                    Console.WriteLine(cb.Message);
                    return Task.FromResult(0);
                }).ExecuteAsync();
            if (!string.IsNullOrEmpty(token.AccessToken))
            {
                await CallGraphApi(token.AccessToken);
            }
        }
        public override async Task DemoWithoutMSALAsync()
        {
            // If you want to register your electronic tooth brush (if it has some means to show a code)
            var code = await GetCodeAsync();
            ResponseToken token = null;
            do
            {
                token = await GetTokenAsync(code);
                await Task.Delay(10000);
            }
            while (token == null || string.IsNullOrEmpty(token.Token));
            Console.Clear();
            if (!string.IsNullOrEmpty(token.Token))
            {
                await CallGraphApi(token.Token);
            }
        }

        private async Task<string> GetCodeAsync()
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"client_id={ClientID}");
            bodyBuilder.Append($"&scope={string.Join(' ', Scopes)}");

            var content = new StringContent(bodyBuilder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            
            var response = await Client.PostAsync(new Uri($"{TenantUrl}devicecode?{bodyBuilder}"), content);
            return await ExtractDeviceTokenAsync(response);
        }

        private async Task<ResponseToken> GetTokenAsync(string code)
        {
            var bodyBuilder = new StringBuilder();
            bodyBuilder.Append($"client_id={ClientID}");
            bodyBuilder.Append($"&device_code={code}");
            bodyBuilder.Append($"&grant_type=urn:ietf:params:oauth:grant-type:device_code");

            var content = new StringContent(bodyBuilder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await Client.PostAsync($"{TenantUrl}token", content);
            return await ExtractTokenAsync(response);
        }
    }
}
