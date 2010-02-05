using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_PlayerDisconnect(ClsPlayer player, string reason)
        {
            this.Raise<PlayerDisconnectEventArgs>(Events.PlayerDisconnect, this, new PlayerDisconnectEventArgs(player, reason));
        }

        event EventHandler<PlayerDisconnectEventArgs> IEvents.PlayerDisconnect
        {
            add { this.Attach<PlayerDisconnectEventArgs>(Events.PlayerDisconnect, value, this._anchor.PlayerDisconnect_Attach); }
            remove { this.Detach<PlayerDisconnectEventArgs>(Events.PlayerDisconnect, value, this._anchor.PlayerDisconnect_Detach); }
        }
    }
}
