using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ServerActivate(int maxclient)
        {
            this.Raise(Events.ServerActivate, this, new ServerActivateEventArgs(maxclient));
        }

        event EventHandler<ServerActivateEventArgs> IEvents.ServerActivate
        {
            add { this.Attach(Events.ServerActivate, value, this._anchor.ServerActivate_Attach); }
            remove { this.Detach(Events.ServerActivate, value, this._anchor.ServerActivate_Detach); }
        }
    }
}
