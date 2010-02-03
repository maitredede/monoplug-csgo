using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSServerPlugin
{
    internal interface INativeMethods
    {
        ERROR ts3server_initServerLib(ref server_callback_struct arg0, LogTypes arg1, string arg2);
        ERROR ts3server_getServerLibVersion(out IntPtr arg0);
        ERROR ts3server_freeMemory(IntPtr arg0);
        ERROR ts3server_destroyServerLib();
        ERROR ts3server_createVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out UInt64 serverID);
        ERROR ts3server_getGlobalErrorMessage(uint errorcode, out IntPtr errormessage);
        ERROR ts3server_getVirtualServerKeyPair(UInt64 serverID, out IntPtr result);
        ERROR ts3server_setVirtualServerVariableAsString(UInt64 serverID, VirtualServerProperties flag, string result);
        ERROR ts3server_flushVirtualServerVariable(UInt64 serverID);
        ERROR ts3server_stopVirtualServer(UInt64 serverID);
    }
}
