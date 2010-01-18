using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Arguments for events related to a client
    /// </summary>
    public class ClientEventArgs : EventArgs
    {
        private readonly ClsPlayer _player;

        /// <summary>
        /// Client for the event
        /// </summary>
        public ClsPlayer Client { get { return this._player; } }

        internal ClientEventArgs(ClsPlayer player)
        {
            this._player = player;
        }
    }
}
