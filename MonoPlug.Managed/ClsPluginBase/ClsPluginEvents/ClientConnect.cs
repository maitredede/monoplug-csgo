using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ClientConnect(ClsPlayer player)
        {
            this.Raise(Events.ClientConnect, this, new ClientEventArgs(player));
        }

        event EventHandler<ClientEventArgs> IEvents.ClientConnect
        {
            add { this.Attach(Events.ClientConnect, value, this._anchor.ClientConnect_Attach); }
            remove { this.Detach(Events.ClientConnect, value, this._anchor.ClientConnect_Detach); }
        }
    }
}
