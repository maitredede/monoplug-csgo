using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Arguments of event ServerActivate
    /// </summary>
    public sealed class ServerActivateEventArgs : EventArgs
    {
        private readonly int _maxclients;

        internal ServerActivateEventArgs(int maxclients)
            : base()
        {
            this._maxclients = maxclients;
        }

        /// <summary>
        /// Max clients
        /// </summary>
        public int MaxClients { get { return this._maxclients; } }
    }
}
