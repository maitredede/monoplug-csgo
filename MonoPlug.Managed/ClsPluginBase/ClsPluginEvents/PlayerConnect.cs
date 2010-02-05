using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_PlayerConnect(ClsPlayer player)
        {
            this.Raise<ClientEventArgs>(Events.PlayerConnect, this, new ClientEventArgs(player));
        }

        event EventHandler<ClientEventArgs> IEvents.PlayerConnect
        {
            add { this.Attach<ClientEventArgs>(Events.PlayerConnect, value, this._anchor.PlayerConnect_Attach); }
            remove { this.Detach<ClientEventArgs>(Events.PlayerConnect, value, this._anchor.PlayerConnect_Detach); }
        }
    }
}
