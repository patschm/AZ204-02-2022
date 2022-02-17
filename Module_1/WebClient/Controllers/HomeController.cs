using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Controllers
{
    public class HomeController: Controller
    {
        private ILogger<HomeController> _logger;
        private IHttpClientFactory _clientFactory;
        public HomeController(IHttpClientFactory factory, ILogger<HomeController> logger)
        {
            _logger = logger;
            _clientFactory = factory;
        }
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("productgroups");
            var result = await client.GetStringAsync("");
            if (!string.IsNullOrEmpty(result))
            {
                return View(JsonConvert.DeserializeObject<List<ProductGroup>>(result));
            }
            return View();
        }
        public async Task<IActionResult> Products(int  id)
        {
            var client = _clientFactory.CreateClient("productgroups");
            var result = await client.GetStringAsync($"products/{id}");
            if (!string.IsNullOrEmpty(result))
            {
                return View(JsonConvert.DeserializeObject<List<Product>>(result));
            }
            return View();
        }
    }
}