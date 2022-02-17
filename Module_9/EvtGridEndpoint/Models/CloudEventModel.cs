using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvtGridEndpoint.Models
{
    public class CloudEventModel
    {
        public string EventId { get; set; }
        public string CloudEventVersion { get; set; }
        public string EventType { get; set; }
        public string EventTypeVersion { get; set; }
        public string Source { get; set; }
        public string EventTime { get; set; }
        public dynamic Data { get; set; }
    }
}
