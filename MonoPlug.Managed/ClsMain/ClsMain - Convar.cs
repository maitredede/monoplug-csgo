using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void Raise_ConVarChange(UInt64 nativeID)
        {
            this._lckConCommandBase.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (this._conCommandBase.ContainsKey(nativeID))
                {
                    ClsConCommandBase item = this._conCommandBase[nativeID];
                    if (item is ClsConVar)
                    {
                        ClsConVar cvar = (ClsConVar)item;
                        cvar.RaiseValueChanged();
                    }
                }
            }
            finally
            {
                this._lckConCommandBase.ReleaseReaderLock();
            }
        }

        public ClsConVar RegisterConvar(ClsPluginBase plugin, string name, string help, FCVAR flags, string defaultValue)
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

                ClsConVar convar;
                if (plugin == null)
                {
                    convar = new ClsConVar(this._msg, this._thPool, plugin, name, help, flags, this._cvarValue, defaultValue);
                }
                else
                {
                    convar = plugin.Proxy.CreateConVar(this._msg, this._thPool, plugin, name, help, flags, this._cvarValue, defaultValue);
                }

                LockCookie cookie = this._lckConCommandBase.UpgradeToWriterLock(Timeout.Infinite);
                try
                {
                    UInt64 nativeId = this.InterThreadCall<UInt64, ClsConVar>(this.RegisterConvar_Call, convar);
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

        private UInt64 RegisterConvar_Call(ClsConVar convar)
        {
            return NativeMethods.Mono_RegisterConVar(convar.Name, convar.Help, (int)convar.Flags, convar.DefaultValue);
        }

        public void UnregisterConvar(ClsPluginBase plugin, ClsConVar var)
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
