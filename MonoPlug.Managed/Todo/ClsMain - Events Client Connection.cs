using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        //private static readonly object _evtToken_ClientPutInServer = new object();
        //private static readonly object _evtToken_ClientDisconnect = new object();

        void IEngine.ClientPutInServer_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ClientPutInServer, null);
        }

        void IEngine.ClientPutInServer_Remove(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ClientPutInServer, null);
        }

        internal void Raise_ClientPutInServer(ClsPlayer player)
        {
            this.AddPlayer(player);

            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(Events.ClientPutInServer))
                {
                    lst = new List<ClsPluginBase>(this._events[Events.ClientPutInServer]);
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
                    if (this._events.ContainsKey(Events.ClientDisconnect))
                    {
                        lst = new List<ClsPluginBase>(this._events[Events.ClientDisconnect]);
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
        }
    }
}
