using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvtGridEndpoint.Hubs
{
    public interface ISender
    {
        Task SendAsync(string evt, string id, string type, string subject, string evtTime, string content);
    }
}
