using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ServerSpawn(string hostname, string address, string port, string game, string mapname, int maxplayers, string os, bool dedicated, bool password)
        {
            this.Raise(Events.ServerSpawn, this, new ServerSpawnEventArgs(hostname, address, port, game, mapname, maxplayers, os, dedicated, password));
        }

        event EventHandler<ServerSpawnEventArgs> IEvents.ServerSpawn
        {
            add { this.Attach<ServerSpawnEventArgs>(Events.ServerSpawn, value, this._anchor.ServerSpawn_Attach); }
            remove { this.Detach<ServerSpawnEventArgs>(Events.ServerSpawn, value, this._anchor.ServerSpawn_Detach); }
        }
    }
}
