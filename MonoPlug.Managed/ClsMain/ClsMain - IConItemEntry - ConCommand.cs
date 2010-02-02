using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        InternalConCommand IConItemEntry.RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete, bool async)
        {
#if DEBUG
            Check.NonNull("code", code);
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.ValidFlags(flags, "flags");
#endif

            this._lckConCommandBase.AcquireReaderLock(Timeout.Infinite);
            try
            {
                //Check if command exists
                foreach (UInt64 id in this._conCommandBase.Keys)
                {
                    if (this._conCommandBase[id].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //Command exists in managed
                        return null;
                    }
                }

                InternalConCommand icmd = new InternalConCommand(name, help, flags, code, complete, async);

                //Try to create it in native
                LockCookie cookie = this._lckConCommandBase.UpgradeToWriterLock(Timeout.Infinite);
                try
                {
                    UInt64 nativeId = this.InterThreadCall<UInt64, InternalConCommand>(this.RegisterConCommand_Call, icmd);
                    if (nativeId > 0)
                    {
                        icmd.SetId(nativeId);
                        this._conCommandBase.Add(nativeId, icmd);
                        return icmd;
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

        private UInt64 RegisterConCommand_Call(InternalConCommand cmd)
        {
            return NativeMethods.Mono_RegisterConCommand(cmd.Name, cmd.Help, (int)cmd.Flags, cmd.Execute, cmd.Complete);
        }

        void IConItemEntry.UnregisterConCommand(InternalConCommand command)
        {
#if DEBUG
            Check.NonNull("command", command);
#endif
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
