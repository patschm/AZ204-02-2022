using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebSite
{
    public class Startup
    {
        private string baseAddress = "https://ps-datasvc.azurewebsites.net/"; 
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(opts=> { 
                opts.ConnectionString = "InstrumentationKey=6f2b25a4-adf9-47a0-810a-755551296338"; 
            });

            services.AddHttpClient("data", opts => {
                opts.BaseAddress = new Uri($"{baseAddress}");
            }).SetHandlerLifetime(TimeSpan.FromMinutes(10)); ;

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
