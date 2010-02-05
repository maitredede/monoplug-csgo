using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ServerShutdown(string reason)
        {
            this.Raise(Events.ServerShutdown, this, new ReasonEventArgs(reason));
        }

        event EventHandler<ReasonEventArgs> IEvents.ServerShutdown
        {
            add { this.Attach<ReasonEventArgs>(Events.ServerShutdown, value, this._anchor.ServerShutdown_Attach); }
            remove { this.Detach<ReasonEventArgs>(Events.ServerShutdown, value, this._anchor.ServerShutdown_Detach); }
        }
    }
}
