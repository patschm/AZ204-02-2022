using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebSite
{
    public class Startup
    {
        private string connectionString = "ps-cash.redis.cache.windows.net:6380,password=6KKJmOkPW0mfOiKibHrE5pbV3F1OuOg8eAzCaGRwCLs=,ssl=True,abortConnect=False";
        private string baseAddress = "https://localhost:5001/";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMemoryCache();
            // From package: Microsoft.Extensions.Caching.StackExchangeRedis
            // "scan 0" to see all entries in redis
            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration = connectionString;
                opt.InstanceName = nameof(WebSite);
            });
            //services.AddDistributedMemoryCache();

            services.AddSession();

            services.AddHttpClient("data", opts => {
                opts.BaseAddress = new Uri($"{baseAddress}");          
            }).SetHandlerLifetime(TimeSpan.FromMinutes(10));;
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
