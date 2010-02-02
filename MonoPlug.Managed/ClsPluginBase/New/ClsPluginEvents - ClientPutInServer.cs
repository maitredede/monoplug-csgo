using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ClientPutInServer(ClsPlayer player)
        {
            this.Raise(Events.ClientPutInServer, new ClientEventArgs(player));
        }

        event EventHandler<ClientEventArgs> IEvents.ClientPutInServer
        {
            add { this.Attach(Events.ClientPutInServer, value, this._anchor.ClientPutInServer_Add); }
            remove { this.Detach(Events.ClientPutInServer, value, this._anchor.ClientPutInServer_Remove); }
        }
    }
}
