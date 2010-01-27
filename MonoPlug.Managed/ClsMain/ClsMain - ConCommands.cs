
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal ClsConCommand RegisterConCommand(ClsPluginBase plugin, string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete)
        {
            Check.NonNull("code", code);
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.ValidFlags(flags, "flags");

            this._lckConCommandBase.AcquireReaderLock(Timeout.Infinite);
            try
            {
                //Check if command exists
                string nup = name.ToUpperInvariant();
                foreach (UInt64 id in this._conCommandBase.Keys)
                {
                    if (this._conCommandBase[id].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Command exists in managed
                        return null;
                    }
                }

                ConCommandData cmdData = new ConCommandData(plugin, name, help, flags, code, complete);
                //Try to create it in native
                LockCookie cookie = this._lckConCommandBase.UpgradeToWriterLock(Timeout.Infinite);
                try
                {
                    UInt64 nativeId = this.InterThreadCall<UInt64, ConCommandData>(this.RegisterConCommand_Call, cmdData);
                    if (nativeId > 0)
                    {
                        cmdData.NativeID = nativeId;
                        ClsConCommand cmd = new ClsConCommand(cmdData);
                        this._conCommandBase.Add(nativeId, cmd);
                        return cmd;
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    this._lckConCommandBase.DowngradeFromWriterLock(ref cookie);
                }
            }
            finally
            {
                this._lckConCommandBase.ReleaseReaderLock();
            }
        }

        private UInt64 RegisterConCommand_Call(ConCommandData entry)
        {
            return NativeMethods.Mono_RegisterConCommand(entry.Name, entry.Help, (int)entry.Flags, entry.Code, entry.Complete);
        }

        internal void UnregisterConCommand(ClsPluginBase plugin, ClsConCommand command)
        {
            this._lckConCommandBase.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (this._conCommandBase.ContainsKey(command.NativeID))
                {
                    this.InterThreadCall<object, UInt64>(this.UnregisterConCommand_Call, command.NativeID);
                    this._conCommandBase.Remove(command.NativeID);
                }
            }
            finally
            {
                this._lckConCommandBase.ReleaseWriterLock();
            }
        }

        private object UnregisterConCommand_Call(UInt64 nativeId)
        {
            NativeMethods.Mono_UnregisterConCommand(nativeId);
            return null;
        }
    }
}
