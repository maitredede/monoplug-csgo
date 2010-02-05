using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace MonoPlug
{
    partial class NativeMethods
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_server_spawn();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_server_spawn();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_server_shutdown();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_server_shutdown();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_player_connect();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_player_connect();

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventAttach_player_disconnect();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void Mono_EventDetach_player_disconnect();
    }
}
