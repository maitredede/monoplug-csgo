using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class NativeEventData
    {
        public GameEvent Event;
        public int ArgsCount;
    }
}
