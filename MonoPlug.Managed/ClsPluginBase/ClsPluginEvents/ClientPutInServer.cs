﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ClientPutInServer(ClsPlayer player)
        {
            this.Raise(Events.ClientPutInServer, this, new ClientEventArgs(player));
        }

        event EventHandler<ClientEventArgs> IEvents.ClientPutInServer
        {
            add { this.Attach(Events.ClientPutInServer, value, this._anchor.ClientPutInServer_Attach); }
            remove { this.Detach(Events.ClientPutInServer, value, this._anchor.ClientPutInServer_Detach); }
        }
    }
}
