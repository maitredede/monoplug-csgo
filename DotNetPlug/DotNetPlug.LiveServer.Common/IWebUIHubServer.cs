using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.LiveServer
{
    public interface IWebUIHubServer
    {
        Task Hello(string serverId);
    }
}
