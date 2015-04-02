using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class ServerActivateEventArgs : EventArgs
    {
        internal ServerActivateEventArgs()
        {
        }

        public int ClientMax { get; internal set; }
    }
}
