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
            this._pool.QueueUserWorkItem(this.Raise_ConVarChange_Thread, nativeID);
        }

        private void Raise_ConVarChange_Thread(UInt64 nativeID)
        {
            //#if DEBUG
            //            this.DevMsg("Threaded ConvarChangedRaise enter...\n");
            //            try
            //            {
            //#endif
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
            //#if DEBUG
            //            }
            //            catch (Exception ex)
            //            {
            //                this.Warning(ex);
            //            }
            //            finally
            //            {
            //                this.DevMsg("Threaded ConvarChangedRaise exit...\n");
            //            }
            //#endif
        }

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
                        ClsConVar convar;
                        if (plugin == null)
                        {
                            //from main
                            convar = new ClsConVar(this, data);
                        }
                        else
                        {
                            convar = plugin.Proxy.CreateConVar(this, data);
                        }
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


        string IConVarValue.GetValueString(UInt64 nativeID)
        {
#if DEBUG
            this.DevMsg("IConVarValue.GetValueString (enter)\n");
            try
            {
#endif
                return this.InterThreadCall<string, UInt64>(this.ConVar_GetString, nativeID);
#if DEBUG
            }
            finally
            {
                this.DevMsg("IConVarValue.GetValueString (exit)\n");
            }
#endif
        }

        private string ConVar_GetString(UInt64 nativeID)
        {
#if DEBUG
            this.DevMsg("ConVar_GetString (enter)\n");
            try
            {
#endif
                return NativeMethods.Mono_Convar_GetString(nativeID);
#if DEBUG
            }
            finally
            {
                this.DevMsg("ConVar_GetString.GetValueString (exit)\n");
            }
#endif
        }

        void IConVarValue.SetValueString(ulong nativeID, string value)
        {
            //#if DEBUG
            //            this.DevMsg("IConVarValue.SetValueString (enter)\n");
            //            try
            //            {
            //#endif
            this.InterThreadCall<object, CTuple<UInt64, string>>(this.ConVar_SetString, new CTuple<UInt64, string>(nativeID, value));
            //#if DEBUG
            //            }
            //            finally
            //            {
            //                this.DevMsg("IConVarValue.SetValueString (exit)\n");
            //            }
            //#endif
        }

        private object ConVar_SetString(CTuple<UInt64, string> data)
        {
            //#if DEBUG
            //            this.DevMsg("ConVar_SetString (enter)\n");
            //            try
            //            {
            //#endif
            NativeMethods.Mono_Convar_SetString(data.Item1, data.Item2);
            return null;
            //#if DEBUG
            //            }
            //            finally
            //            {
            //                this.DevMsg("ConVar_SetString (exit)\n");
            //            }
            //#endif
        }

        bool IConVarValue.GetValueBool(ulong nativeID)
        {
            //#if DEBUG
            //            this.DevMsg("IConVarValue.GetValueBool (enter)\n");
            //            try
            //            {
            //#endif
            return this.InterThreadCall<bool, UInt64>(this.ConVar_GetBool, nativeID);
            //#if DEBUG
            //            }
            //            finally
            //            {
            //                this.DevMsg("IConVarValue.GetValueBool (exit)\n");
            //            }
            //#endif
        }

        private bool ConVar_GetBool(UInt64 nativeID)
        {
            //#if DEBUG
            //            this.DevMsg("ConVar_GetString (enter)\n");
            //            try
            //            {
            //#endif
            return NativeMethods.Mono_Convar_GetBoolean(nativeID);
            //#if DEBUG
            //            }
            //            finally
            //            {
            //                this.DevMsg("ConVar_GetString.GetValueString (exit)\n");
            //            }
            //#endif
        }


        void IConVarValue.SetValueBool(ulong nativeID, bool value)
        {
            //#if DEBUG
            //            this.DevMsg("IConVarValue.SetValueBool (enter)\n");
            //            try
            //            {
            //#endif
            this.InterThreadCall<object, CTuple<UInt64, bool>>(this.ConVar_SetBool, new CTuple<UInt64, bool>(nativeID, value));
            //#if DEBUG
            //            }
            //            finally
            //            {
            //                this.DevMsg("IConVarValue.SetValueBool (exit)\n");
            //            }
            //#endif
        }

        private object ConVar_SetBool(CTuple<UInt64, bool> data)
        {
            //#if DEBUG
            //            this.DevMsg("ConVar_SetBool (enter)\n");
            //            try
            //            {
            //#endif
            NativeMethods.Mono_Convar_SetBoolean(data.Item1, data.Item2);
            return null;
            //#if DEBUG
            //            }
            //            finally
            //            {
            //                this.DevMsg("ConVar_SetBool (exit)\n");
            //            }
            //#endif
        }
    }
}
