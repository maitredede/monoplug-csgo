using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class GameEventEventArgs : EventArgs
    {
        internal GameEventEventArgs()
        {

        }

        public GameEvent Event { get; internal set; }
        public dynamic Args { get; internal set; }
    }
}
