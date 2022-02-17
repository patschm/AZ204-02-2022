using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using EvtGridEndpoint.Hubs;
using EvtGridEndpoint.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EvtGridEndpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridController : ControllerBase
    {
        private readonly IHubContext<MyHub> _myhub;
        private readonly ILogger<GridController> _logger;

        public GridController(IHubContext<MyHub> hub, ILogger<GridController> logger)
        {
            _myhub = hub;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using (StreamReader rdr = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var body = await rdr.ReadToEndAsync();
               
                // Check the type of event. SubscriptionValidation is sent when registering a subscription to a topic
                if (Request.Headers["aeg-event-type"].FirstOrDefault() == "SubscriptionValidation")
                {
                    // Exchange validation token to check if the subscription belongs to the endpoint
                    return await ValidateSubscriptionAsync(body);
                }
                else if (Request.Headers["aeg-event-type"].FirstOrDefault() == "Notification")
                {
                    // Custom event (List of events) or cloud event (One at a time)
                    _logger.LogInformation(body);
                    if (IsCloudEvent(body))
                    {
                        return await HandleCloudEventAsync(body);
                    }
                    return await HandleGridEventAsync(body);

                }
                return BadRequest();
            }
        }
        private async Task<IActionResult> HandleGridEventAsync(string body)
        {
            EventGridEvent[] events = EventGridEvent.ParseMany(BinaryData.FromString(body));
            //var details = JsonConvert.DeserializeObject<List<GridEventModel>>(body).First();
            //string data = JsonConvert.SerializeObject(details.Data);
            foreach (var evt in events)
            {
                string data = evt.Data.ToString();
                await _myhub.Clients.All.SendAsync(
                    "gridupdate",
                    evt.Id,
                    evt.EventType,
                    evt.Subject,
                    evt.EventTime,
                    data,
                    body
                );
            }
            return Ok();
        }
        private async Task<IActionResult> HandleCloudEventAsync(string body)
        {
            CloudEvent[] events = CloudEvent.ParseMany(BinaryData.FromString(body));
            //var details = JsonConvert.DeserializeObject<CloudEventModel>(body);
            foreach (var evt in events)
            {
                await this._myhub.Clients.All.SendAsync(
                  "gridupdate",
                  evt.Id,
                  evt.Type,
                  evt.Subject,
                  evt.Time,
                  body
              );
            }
            return Ok();
        }
        private async Task<IActionResult> ValidateSubscriptionAsync(string body)
        {
            var gridEvent = JsonConvert.DeserializeObject<List<GridEventModel>>(body).First();
            await _myhub.Clients.All.SendAsync(
                "gridupdate",
                gridEvent.Id,
                gridEvent.EventType,
                gridEvent.Subject,
                gridEvent.EventTime,
                JsonConvert.SerializeObject(gridEvent.Data),
                body);

            var validationCode = gridEvent.Data["validationCode"];
            return new JsonResult(new
            {
                validationResponse = validationCode
            });
        }
        private static bool IsCloudEvent(string jsonContent)
        {
            try
            {
                var eventData = JObject.Parse(jsonContent);
                var version = eventData["cloudEventsVersion"].Value<string>();
                return !string.IsNullOrEmpty(version);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }
    }
}