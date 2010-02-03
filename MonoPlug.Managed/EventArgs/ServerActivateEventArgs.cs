using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    public sealed class ServerActivateEventArgs : EventArgs
    {
        private readonly int _maxclients;

        internal ServerActivateEventArgs(int maxclients)
            : base()
        {
            this._maxclients = maxclients;
        }

        public int MaxClients { get { return this._maxclients; } }
    }
}
