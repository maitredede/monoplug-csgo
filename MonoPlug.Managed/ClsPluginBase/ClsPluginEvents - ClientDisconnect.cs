using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ClientDisconnect(ClsPlayer player)
        {
            this.Raise(Events.ClientDisconnect, this, new ClientEventArgs(player));
        }

        event EventHandler<ClientEventArgs> IEvents.ClientDisconnect
        {
            add { this.Attach(Events.ClientDisconnect, value, this._anchor.ClientDisconnect_Attach); }
            remove { this.Detach(Events.ClientDisconnect, value, this._anchor.ClientDisconnect_Detach); }
        }
    }
}
