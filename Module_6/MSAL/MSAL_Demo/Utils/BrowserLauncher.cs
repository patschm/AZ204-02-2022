using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MSAL_Demo
{
    public class BrowserLauncher
    {
        public async Task<Uri> StartBrowserPlatformAsync(Uri uri, Uri redirectUrl)
        {
            await StartBrowserAsync(uri);
            return await ListenForRedirectAsync(redirectUrl);
        }

        private async Task<Uri> ListenForRedirectAsync(Uri redirectUrl)
        {
            // The browser will receive a redirect to a localhost socket (which obviously doesn't exist)
            // Create a socket for the browser to redirect to
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add($"http://localhost:{redirectUrl.Port}/");
            //httpListener.Prefixes.Add($"https://localhost:{redirectUrl.Port + 1}/");
            httpListener.IgnoreWriteExceptions = true;
            httpListener.Start();

            HttpListenerContext context = await httpListener.GetContextAsync();
            var res = context.Request.Url;
           
            httpListener.Abort();
            return res;
        }

        private Task StartBrowserAsync(Uri uri)
        {
            // Start a browser
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = uri.AbsoluteUri,
                UseShellExecute = true
            };

            Process.Start(psi);
            return Task.CompletedTask;
        }
    }
}
