using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace DotNetPlug.LiveServer.Web
{
    public sealed class PluginServerHub : Hub, IPluginHubServer
    {
        private static Dictionary<string, string> m_connectedServers_Id_Connex = new Dictionary<string, string>();
        private static Dictionary<string, string> m_connectedServers_Connex_Id = new Dictionary<string, string>();

        public async Task<bool> Hello(string serverId, string serverKey)
        {
            //Id/Key validation
            if (serverId != "Demo" || serverKey != "DemoKey")
                return false;

            lock (m_connectedServers_Id_Connex)
            {
                //Check not already connected
                if (m_connectedServers_Id_Connex.ContainsKey(serverId))
                {
                    return false;
                }

                m_connectedServers_Id_Connex.Add(serverId, this.Context.ConnectionId);
                m_connectedServers_Connex_Id.Add(this.Context.ConnectionId, serverId);
            }
            return true;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            lock (m_connectedServers_Id_Connex)
            {
                if (m_connectedServers_Connex_Id.ContainsKey(this.Context.ConnectionId))
                {
                    string serverId = m_connectedServers_Connex_Id[this.Context.ConnectionId];

                    m_connectedServers_Id_Connex.Remove(serverId);
                    m_connectedServers_Connex_Id.Remove(this.Context.ConnectionId);
                }
            }
            return base.OnDisconnected(stopCalled);
        }


        public async Task RaiseEvent(GameEventData args)
        {
            string serverId;
            lock (m_connectedServers_Id_Connex)
            {
                if (!m_connectedServers_Connex_Id.ContainsKey(this.Context.ConnectionId))
                    return;

                serverId = m_connectedServers_Connex_Id[this.Context.ConnectionId];
            }
            await WebUIHub.RaiseEventInternal(serverId, args);
        }

        public async Task SetPlayers(PlayerData[] players)
        {
            string serverId;
            lock (m_connectedServers_Id_Connex)
            {
                //Check not already connected
                if (!m_connectedServers_Connex_Id.ContainsKey(this.Context.ConnectionId))
                {
                    return;
                }
                serverId = m_connectedServers_Connex_Id[this.Context.ConnectionId];
            }
            await WebUIHub.SetPlayers(serverId, players);

        }
    }
}