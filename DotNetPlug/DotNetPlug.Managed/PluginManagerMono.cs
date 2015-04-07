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

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void Log(string message);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void ExecuteCommand(string command, out string output, out int length);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern int RegisterCommand(byte[] commandUTF8, byte[] descriptionUTF8, int iFlags, int id);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void UnregisterCommand(int id);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void ShowMOTD(int playerId, byte[] titleUTF8, byte[] msgUTF8, int motdType, byte[] cmdUTF8);
    }
}
