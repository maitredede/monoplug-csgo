using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DotNetPlug.LiveServer.Web
{
    public class WebUIHub : Hub<IWebUIHubClient>, IWebUIHubServer
    {
        internal static Task RaiseEventInternal(string serverId, GameEventData e)
        {
            return GlobalHost.ConnectionManager.GetHubContext<WebUIHub, IWebUIHubClient>().Clients.Group(serverId).RaiseEvent(serverId, e);
        }

        public async Task Hello(string serverId)
        {
            await this.Groups.Add(this.Context.ConnectionId, serverId);
        }
    }
}