using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using WebSite.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IDistributedCache _redis;
        private IHttpClientFactory _clients;
        private IWebHostEnvironment _env;

        public HomeController(IHttpClientFactory factory, IDistributedCache redis, IWebHostEnvironment env, ILogger<HomeController> logger)
        {
            _clients = factory;
            _redis = redis;
            _logger = logger;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var client =  _clients.CreateClient("data");
            var result = await client.GetAsync("productgroups");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return View(JsonConvert.DeserializeObject<List<ProductGroup>>(json));
            }
            return RedirectToAction("Error");
        }

        public async Task<IActionResult> Products(int id)
        {
            var key = $"products_{id}";
            await _redis.RefreshAsync(key); // Sliding Expiration   
            var products = await _redis.GetStringAsync(key);
            if (string.IsNullOrEmpty(products))
            {
                await Task.Delay(20000);
                var client = _clients.CreateClient("data");
                var result = await client.GetAsync($"productgroups/products/{id}");
                if (result.IsSuccessStatusCode)
                {
                    products= await result.Content.ReadAsStringAsync();
                    
                    await _redis.SetStringAsync(key, products, 
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120) });
                }
            }
       
            return View(JsonConvert.DeserializeObject<List<Product>>(products));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
