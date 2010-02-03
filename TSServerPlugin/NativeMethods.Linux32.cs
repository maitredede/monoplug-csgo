using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TSServerPlugin
{
    internal sealed class NativeMethods_Linux32 : INativeMethods
    {
        internal const string lib = "libts3server_linux_x86";

        [DllImport(lib, EntryPoint = "ts3server_initServerLib")]
        private static extern ERROR initServerLib(ref server_callback_struct arg0, LogTypes arg1, string arg2);

        [DllImport(lib, EntryPoint = "ts3server_getServerLibVersion")]
        private static extern ERROR getServerLibVersion(out IntPtr arg0);

        [DllImport(lib, EntryPoint = "ts3server_freeMemory")]
        private static extern ERROR freeMemory(IntPtr arg0);

        [DllImport(lib, EntryPoint = "ts3server_destroyServerLib")]
        private static extern ERROR destroyServerLib();

        [DllImport(lib, EntryPoint = "ts3server_createVirtualServer")]
        private static extern ERROR createVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out UInt64 serverID);

        [DllImport(lib, EntryPoint = "ts3server_getGlobalErrorMessage")]
        private static extern ERROR getGlobalErrorMessage(uint errorcode, out IntPtr errormessage);

        [DllImport(lib, EntryPoint = "ts3server_getVirtualServerKeyPair")]
        private static extern ERROR getVirtualServerKeyPair(UInt64 serverID, out IntPtr result);

        [DllImport(lib, EntryPoint = "ts3server_setVirtualServerVariableAsString")]
        private static extern ERROR setVirtualServerVariableAsString(UInt64 serverID, VirtualServerProperties flag, string result);

        [DllImport(lib, EntryPoint = "ts3server_flushVirtualServerVariable")]
        private static extern ERROR flushVirtualServerVariable(UInt64 serverID);

        [DllImport(lib, EntryPoint = "ts3server_stopVirtualServer")]
        private static extern ERROR stopVirtualServer(UInt64 serverID);

        public ERROR ts3server_initServerLib(ref server_callback_struct arg0, LogTypes arg1, string arg2)
        {
            return initServerLib(ref arg0, arg1, arg2);
        }

        public ERROR ts3server_getServerLibVersion(out IntPtr arg0)
        {
            return getServerLibVersion(out arg0);
        }

        public ERROR ts3server_freeMemory(IntPtr arg0)
        {
            return freeMemory(arg0);
        }

        public ERROR ts3server_destroyServerLib()
        {
            return destroyServerLib();
        }

        public ERROR ts3server_createVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out ulong serverID)
        {
            return createVirtualServer(serverPort, ip, serverName, serverKeyPair, serverMaxClients, out serverID);
        }

        public ERROR ts3server_getGlobalErrorMessage(uint errorcode, out IntPtr errormessage)
        {
            return getGlobalErrorMessage(errorcode, out errormessage);
        }

        public ERROR ts3server_getVirtualServerKeyPair(ulong serverID, out IntPtr result)
        {
            return getVirtualServerKeyPair(serverID, out result);
        }

        public ERROR ts3server_setVirtualServerVariableAsString(ulong serverID, VirtualServerProperties flag, string result)
        {
            return setVirtualServerVariableAsString(serverID, flag, result);
        }

        public ERROR ts3server_flushVirtualServerVariable(ulong serverID)
        {
            return flushVirtualServerVariable(serverID);
        }

        public ERROR ts3server_stopVirtualServer(ulong serverID)
        {
            return stopVirtualServer(serverID);
        }
    }
}
