using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class NativeEventData
    {
        private const int NATIVE_EVENT_ARGS_MAX = 16;

        public GameEvent Event;
        public int ArgsCount;
        public NativeEventArgs[] Args;
    }

    [StructLayout(LayoutKind.Sequential)]
    public sealed class NativeEventArgs
    {
        [MarshalAs(UnmanagedType.BStr)]
        public string Name;
        public NativeEventArgType Type;
    }

    public enum NativeEventArgType : int
    {
        Int = 0,
        String = 1,
        Bool = 2
    }
}
