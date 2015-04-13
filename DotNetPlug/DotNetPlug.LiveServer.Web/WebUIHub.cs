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
        private static PlayerData[] s_players = null;

        internal static Task RaiseEventInternal(string serverId, GameEventData e)
        {
            return GlobalHost.ConnectionManager.GetHubContext<WebUIHub, IWebUIHubClient>().Clients.Group(serverId).RaiseEvent(serverId, e);
        }

        internal static Task SetPlayers(string serverId, PlayerData[] players)
        {
            lock (typeof(WebUIHub))
            {
                s_players = players;
            }
            return GlobalHost.ConnectionManager.GetHubContext<WebUIHub, IWebUIHubClient>().Clients.Group(serverId).SetPlayers(serverId, players);
        }

        public async Task Hello(string serverId)
        {
            await this.Groups.Add(this.Context.ConnectionId, serverId);
            PlayerData[] players;
            lock (typeof(WebUIHub))
            {
                players = s_players;
            }
            await this.Clients.Caller.SetPlayers(serverId, players);
        }


    }
}