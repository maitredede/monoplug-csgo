using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public sealed class ClientCommandEventArgs : ClientEventArgs
    {
        private readonly string _command;

        internal ClientCommandEventArgs(ClsPlayer player, string command)
            : base(player)
        {
            this._command = command;
        }

        public string Command { get { return this._command; } }
    }
}
