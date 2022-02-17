
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Entities;

namespace CSHttpClientSample 
{
    static class Program
    {
        static void Main()
        {
            MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }
        static async void MakeRequest()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://ps-aapies.azure-api.net/ds/");
            // Request headers
            client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "20543d1a910b4891ac0359d78cb5e86d");
            var uri = "brands?start=0&count=10";
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                Brand[] brands = JsonConvert.DeserializeObject<Brand[]>(str);

                foreach(Brand b in brands)
                {
                    Console.WriteLine($"{b.Name} ({b.Website})");
                }
            }
        }
    }
}
                                                                                                                                                                                                                                                                                         // }