using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        event EventHandler IEvents.LevelShutdown
        {
            add { this.Attach(Events.LevelShutdown, value, this._anchor.LevelShutdown_Attach); }
            remove { this.Detach(Events.LevelShutdown, value, this._anchor.LevelShutdown_Detach); }
        }

        internal void Raise_LevelShutdown()
        {
            this.RaiseEmpty(Events.LevelShutdown, this._owner);
        }
    }
}
