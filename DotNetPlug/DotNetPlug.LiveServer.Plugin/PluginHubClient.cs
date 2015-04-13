using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.LiveServer
{
    internal sealed class PluginHubClient : IPluginHubServer
    {
        private readonly IHubProxy m_proxy;

        public PluginHubClient(IHubProxy proxy)
        {
            this.m_proxy = proxy;
        }

        public Task<bool> Hello(string serverId, string serverKey)
        {
            return this.m_proxy.Invoke<bool>("hello", serverId, serverKey);
        }

        public Task RaiseEvent(GameEventData args)
        {
            return this.m_proxy.Invoke<bool>("raiseEvent", new object[] { args });
        }

        public Task SetPlayers(PlayerData[] players)
        {
            return this.m_proxy.Invoke("setPlayers", new object[] { players });
        }
    }
}
