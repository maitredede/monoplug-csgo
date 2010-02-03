using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonoPlug
{
    internal static class NativeMethods
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Msg(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Log(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_DevMsg(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Warning(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Error(string msg);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern UInt64 Mono_RegisterConCommand(string name, string description, int flags, ConCommandDelegate code, ConCommandCompleteDelegate complete);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_UnregisterConCommand(UInt64 nativeId);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_AttachConsole();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_DetachConsole();


        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern UInt64 Mono_RegisterConVar(string name, string description, int flags, string defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_UnregisterConVar(UInt64 nativeID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string Mono_Convar_GetString(UInt64 nativeID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Convar_SetString(UInt64 nativeID, string value);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Mono_Convar_GetBoolean(UInt64 nativeID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Convar_SetBoolean(UInt64 nativeID, bool value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_ServerActivate();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_ServerActivate();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_ClientDialogMessage(int client, string title, string message, int a, int r, int g, int b, int level, int time);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_LevelShutdown();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_LevelShutdown();


        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_ClientDisconnect();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_ClientDisconnect();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_ClientPutInServer();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_ClientPutInServer();
    }
}
