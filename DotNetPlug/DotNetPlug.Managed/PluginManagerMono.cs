using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal static class PluginManagerMono
    {
        private const string Internal = "__Internal";

        //[DllImport(Internal, EntryPoint = "Mono_Log")]
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Log(string message);

        //[DllImport(Internal, EntryPoint = "Mono_ExecuteCommand")]
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern string ExecuteCommand(string command);
    }
}
