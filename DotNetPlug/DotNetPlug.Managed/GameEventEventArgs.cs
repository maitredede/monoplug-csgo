using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class GameEventEventArgs : EventArgs
    {
        internal GameEventEventArgs(GameEvent evt, GameEventArgument[] args)
        {
            this.Event = evt;
            this.Args = args;
        }

        public GameEvent Event { get; private set; }
        public GameEventArgument[] Args { get; private set; }
    }
}
