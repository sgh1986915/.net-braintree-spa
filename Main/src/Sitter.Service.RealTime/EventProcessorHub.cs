using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Avocado.Sitter.Service.RealTime
{

    //public class StateEvent
    //{
    //    public string FieldName { get; set; }
    //    public string FieldValue { get; set; }
    //}

    public class EventProcessorHub : Hub
    {
        public void UpdateState(string message)
        {
            Clients.AllExcept(new[] { Context.ConnectionId }).RefreshState(message);
        }
    }
}
