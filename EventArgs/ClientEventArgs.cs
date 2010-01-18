using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    public class ClientEventArgs : EventArgs
    {
        private readonly ClsPlayer _player;

        public ClsPlayer Client { get { return this._player; } }

        internal ClientEventArgs(ClsPlayer player)
        {
            this._player = player;
        }
    }
}
