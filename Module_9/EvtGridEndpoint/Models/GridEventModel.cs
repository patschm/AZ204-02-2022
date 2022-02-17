using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvtGridEndpoint.Models
{
    public class GridEventModel
    {
        public string Id { get; set; }
        public string EventType { get; set; }
        public string Subject { get; set; }
        public DateTime EventTime { get; set; }        
        public string Topic { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
