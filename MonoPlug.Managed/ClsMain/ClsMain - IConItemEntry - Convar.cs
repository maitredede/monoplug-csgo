using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain : IEngineWrapper
    {
        InternalConvar IEngineWrapper.RegisterConvar(string name, string help, FCVAR flags, string defaultValue)
        {
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.ValidFlags(flags, "flags");
            Check.NonNull("defaultValue", defaultValue);

            this._lckConCommandBase.AcquireReaderLock(Timeout.Infinite);
            try
            {
                //Check if command exists
                foreach (UInt64 id in this._conCommandBase.Keys)
                {
                    if (this._conCommandBase[id].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //var exists in managed
                        return null;
                    }
                }

                InternalConvar convar = new InternalConvar(name, help, flags, defaultValue, this._cvarValue);

                LockCookie cookie = this._lckConCommandBase.UpgradeToWriterLock(Timeout.Infinite);
                try
                {
                    UInt64 nativeId = this.InterThreadCall<UInt64, InternalConvar>(this.RegisterConvar_Call, convar);
                    if (nativeId > 0)
                    {
                        convar.SetId(nativeId);
                        this._conCommandBase.Add(nativeId, convar);
                        return convar;
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

        private UInt64 RegisterConvar_Call(InternalConvar convar)
        {
            return NativeMethods.Mono_RegisterConVar(convar.Name, convar.Help, (int)convar.Flags, convar.DefaultValue);
        }

        void IEngineWrapper.UnregisterConvar(InternalConvar var)
        {
            Check.NonNull("var", var);

            this._lckConCommandBase.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (this._conCommandBase.ContainsKey(var.NativeID))
                {
                    this.InterThreadCall<object, UInt64>(this.UnregisterConvar_Call, var.NativeID);
                    this._conCommandBase.Remove(var.NativeID);
                }
            }
            finally
            {
                this._lckConCommandBase.ReleaseWriterLock();
            }
        }

        private object UnregisterConvar_Call(UInt64 nativeID)
        {
            NativeMethods.Mono_UnregisterConVar(nativeID);
            return null;
        }
    }
}
