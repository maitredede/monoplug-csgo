using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public sealed class ClientCommandEventArgs : EventArgs
    {
        private readonly ClsPlayer _player;
        private readonly string _command;

        internal ClientCommandEventArgs(ClsPlayer player, string command)
            : base()
        {
            this._player = player;
            this._command = command;
        }

        public ClsPlayer Player { get { return this._player; } }
        public string Command { get { return this._command; } }
    }
}
