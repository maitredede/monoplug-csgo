using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal ClsConVar RegisterConvar(ClsPluginBase plugin, string name, string help, FCVAR flags, string defaultValue)
        {
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.ValidFlags(flags, "flags");
            Check.NonNull("defaultValue", defaultValue);

            this._lckConCommandBase.AcquireReaderLock(Timeout.Infinite);
            try
            {
                //Check if command exists
                string nup = name.ToUpperInvariant();
                foreach (UInt64 id in this._conCommandBase.Keys)
                {
                    if (this._conCommandBase[id].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //var exists in managed
                        return null;
                    }
                }

                ConVarData data = new ConVarData(plugin, name, help, flags, defaultValue);
                LockCookie cookie = this._lckConCommandBase.UpgradeToWriterLock(Timeout.Infinite);
                try
                {
                    UInt64 nativeId = this.InterThreadCall<UInt64, ConVarData>(this.RegisterConvar_Call, data);
                    if (nativeId > 0)
                    {
                        data.NativeID = nativeId;
                        ClsConVar convar = plugin.Proxy.CreateConVar(this, data);
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

        private UInt64 RegisterConvar_Call(ConVarData data)
        {
            return NativeMethods.Mono_RegisterConVar(data.Name, data.Help, (int)data.Flags, data.DefaultValue);
        }

        internal void Raise_ConVarChange(UInt64 nativeID)
        {
#if DEBUG
            this.DevMsg("Threaded ConvarChangedRaise enter...\n");
            try
            {
#endif
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
#if DEBUG
            }
            catch (Exception ex)
            {
                this.Warning(ex);
            }
            finally
            {
                this.DevMsg("Threaded ConvarChangedRaise exit...\n");
            }
#endif
        }

        internal void UnregisterConvar(ClsPluginBase plugin, ClsConVar var)
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
