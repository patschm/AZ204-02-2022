using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Entities;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        private TelemetryClient _teleClient;
        private ILogger<HomeController> _logger;
        private IHttpClientFactory _clients;

        public HomeController(IHttpClientFactory factory, TelemetryClient client, ILogger<HomeController> logger)
        {
            _teleClient = client;
            _logger = logger;
            _clients = factory;
        }

        public IActionResult Index()
        {
            _teleClient.TrackException(new Exception("Teletubby says ooops"));
            _logger.LogInformation("Information");
            _logger.LogWarning("Warning");
            _logger.LogError("Error");
            _logger.LogCritical("Critical");
            _logger.LogCritical(new DivideByZeroException(), "Ooops");
            return View();
        }

        public async Task<IActionResult> ProductGroups()
        {
            var client = _clients.CreateClient("data");
            var result = await client.GetAsync("productgroups");
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return View(JsonConvert.DeserializeObject<List<ProductGroup>>(json));
            }
            _logger.LogError($"Unable to load data from '{result.RequestMessage.RequestUri}'. {result.StatusCode}");
            return RedirectToAction("Error");
        }

        public async Task<IActionResult> Products(int id)
        {
            var client = _clients.CreateClient("data");
            var result = await client.GetAsync($"productgroups/products/{id}");
            if (result.IsSuccessStatusCode)
            {
                var products = await result.Content.ReadAsStringAsync();
                return View(JsonConvert.DeserializeObject<List<Product>>(products));
            }   
            _logger.LogError($"Unable to load data from '{result.RequestMessage.RequestUri}'. {result.StatusCode}");
            return View("Error");
        }
    }
}
