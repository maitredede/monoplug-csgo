using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal static class Events
    {
        public static readonly object ClientPutInServer = new object();
        public static readonly object ConvarValueChanged = new object();
        public static readonly object ConsoleMessage = new object();
        public static readonly object LevelShutdown = new object();
        public static readonly object ClientDisconnect = new object();
        public static readonly object ServerActivate = new object();
        public static readonly object ServerSpawn = new object();
        public static readonly object ServerShutdown = new object();
        public static readonly object PlayerConnect = new object();
        public static readonly object PlayerDisconnect = new object();
    }
}
