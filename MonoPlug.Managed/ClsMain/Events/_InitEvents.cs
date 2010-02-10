using System;

namespace MonoPlug
{
    partial class ClsMain
    {
        /// <summary>
        /// Init function for main instance 
        /// </summary>
        private void InitEvents()
        {
            NativeMethods.Mono_EventAttach_ClientConnect();
            NativeMethods.Mono_EventAttach_ClientDisconnect();
            NativeMethods.Mono_EventAttach_ClientPutInServer();

            NativeMethods.Mono_EventAttach_player_connect();
            NativeMethods.Mono_EventAttach_player_disconnect();
            NativeMethods.Mono_EventAttach_LevelShutdown();
        }

        private void ShutdownEvents()
        {
            NativeMethods.Mono_EventDetach_ClientConnect();
            NativeMethods.Mono_EventDetach_ClientDisconnect();
            NativeMethods.Mono_EventDetach_ClientPutInServer();

            NativeMethods.Mono_EventDetach_player_connect();
            NativeMethods.Mono_EventDetach_player_disconnect();
            NativeMethods.Mono_EventDetach_LevelShutdown();
        }
    }
}
