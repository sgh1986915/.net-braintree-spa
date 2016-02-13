using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Avocado.Sitter.Service.RealTime
{
    public class EchoHub : Hub
    {
        public void Send(string message)
        {
            Clients.All.Echo(string.Format("{0} -- Echoed from server at {1}! ", message, DateTime.Now));
        }
    }
}
