using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace MonoPlug
{
    internal static class NativeMethods
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Msg(string msg);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Log(string msg);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern UInt64 Mono_RegisterConvar(string name, string description, int flags, string defaultValue);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Mono_UnregisterConvar(UInt64 nativeID);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string Mono_Convar_GetString(UInt64 nativeID);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_Convar_SetString(UInt64 nativeID, string value);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Mono_RegisterConCommand(string name, string description, ConCommandDelegate code, int flags, ConCommandCompleteDelegate complete);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool Mono_UnregisterConCommand(string name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ClsPlayer[] Mono_GetPlayers();
        //Below is not native attached

        [MethodImpl(MethodImplOptions.InternalCall)]
        [Obsolete("To be removed", true)]
        internal static extern string Mono_GetConVarStringValue(UInt64 nativeId);
        [MethodImpl(MethodImplOptions.InternalCall)]
        [Obsolete("To be removed", true)]
        internal static extern void Mono_SetConVarStringValue(UInt64 nativeId, string value);
    }
}
