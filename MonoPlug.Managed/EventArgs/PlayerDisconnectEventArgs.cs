using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Player disconnected
    /// </summary>
    public sealed class PlayerDisconnectEventArgs : ClientEventArgs
    {
        private readonly string _reason;

        /// <summary>
        /// Reason why player was disconnected
        /// </summary>
        public string Reason { get { return this._reason; } }

        internal PlayerDisconnectEventArgs(ClsPlayer player, string reason)
            : base(player)
        {
            this._reason = reason;
        }
    }
}
