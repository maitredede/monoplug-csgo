using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Arguments of ClientCommand event
    /// </summary>
    public sealed class ClientCommandEventArgs : ClientEventArgs
    {
        private readonly string _command;

        internal ClientCommandEventArgs(ClsPlayer player, string command)
            : base(player)
        {
            this._command = command;
        }

        /// <summary>
        /// The command the client has written
        /// </summary>
        public string Command { get { return this._command; } }
    }
}
