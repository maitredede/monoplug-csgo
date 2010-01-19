using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        private static readonly object _evtToken_ClientPutInServer = new object();
        private static readonly object _evtToken_ClientDisconnect = new object();

        internal void ClientPutInServer_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, _evtToken_ClientPutInServer, null);
        }

        internal void ClientPutInServer_Remove(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, _evtToken_ClientPutInServer, null);
        }

        internal void Raise_ClientPutInServer(ClsPlayer player)
        {
            this.AddPlayer(player);

            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(_evtToken_ClientPutInServer))
                {
                    lst = new List<ClsPluginBase>(this._events[_evtToken_ClientPutInServer]);
                }
            }
            if (lst != null && lst.Count > 0)
            {
                ClientEventArgs e = new ClientEventArgs(player);
                foreach (ClsPluginBase plugin in lst)
                {
                    plugin.Raise_ClientPutInServer(this, e);
                }
            }
        }

        internal void Raise_ClientDisconnect(int playerId)
        {
            ClsPlayer player = this.RemovePlayer(playerId);
            if (player != null)
            {
                List<ClsPluginBase> lst = null;
                lock (this._events)
                {
                    if (this._events.ContainsKey(_evtToken_ClientDisconnect))
                    {
                        lst = new List<ClsPluginBase>(this._events[_evtToken_ClientDisconnect]);
                    }
                }
                if (lst != null && lst.Count > 0)
                {
                    ClientEventArgs e = new ClientEventArgs(player);
                    foreach (ClsPluginBase plugin in lst)
                    {
                        plugin.Raise_ClientDisconnect(this, e);
                    }
                }
            }
            //TODO : Raise_ClientDisconnect
        }
    }
}
