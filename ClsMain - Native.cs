using System;
using System.Runtime.CompilerServices;

namespace MonoPlug
{
	partial class ClsMain
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Mono_Msg(string msg);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Mono_RegisterConCommand(string name, string description, ConCommandDelegate code);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Mono_UnregisterConCommand(string name);
	}
}
