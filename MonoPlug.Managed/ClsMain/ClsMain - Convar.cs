using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal ClsConvarMain RegisterConvar(ClsPluginBase plugin, string name, string help, FCVAR flags, string defaultValue)
        {
            Check.NonNullOrEmpty("name", name);
            Check.NonNullOrEmpty("help", help);
            Check.NonNull("defaultValue", defaultValue);
            Check.ValidFlags(flags, "flags");

            ConvarRegisterData data = new ConvarRegisterData();
            data.Name = name;
            data.Help = help;
            data.Flags = flags;
            data.DefaultValue = defaultValue;

            this._lckConvars.AcquireReaderLock(Timeout.Infinite);
            try
            {
                UInt64 nativeID = this.InterThreadCall<UInt64, ConvarRegisterData>(this.RegisterConvar_Call, data);
                if (nativeID > 0)
                {
                    ClsConvarMain var = new ClsConvarMain(this, nativeID);
                    ConVarEntry entry = new ConVarEntry(plugin, var, data);
                    this._convarsList.Add(nativeID, entry);
#if DEBUG
                    this.DevMsg("Registeted convar '{0}' for plugin '{1}'\n", name, plugin ?? (object)"<main>");
#endif
                    return var;
                }
                else
                {
                    this.Warning("Can't register var {0} for plugin {1}\n", name, plugin ?? (object)"<main>");
                    return null;
                }
            }
#if DEBUG
            catch (Exception ex)
            {
                this.Warning(ex);
                return null;
            }
#endif
            finally
            {
                this._lckConvars.ReleaseReaderLock();
            }
        }

        private UInt64 RegisterConvar_Call(ConvarRegisterData data)
        {
            return NativeMethods.Mono_RegisterConvar(data.Name, data.Help, (int)data.Flags, data.DefaultValue);
        }

        internal void ConvarChanged(UInt64 nativeID)
        {
#if DEBUG
            this.DevMsg("ConvarChanged({0}) Enter\n", nativeID);
            try
            {
#endif
                this._lckConvars.AcquireReaderLock(Timeout.Infinite);
                try
                {
                    if (this._convarsList.ContainsKey(nativeID))
                    {
                        ConVarEntry entry = this._convarsList[nativeID];
#if DEBUG
                        this.DevMsg("Queueing event raise\n");
#endif
                        //this._pool.QueueUserWorkItem(this.ConvarChangedRaise, entry);
                        this.ConvarChangedRaise(entry);
#if DEBUG
                        this.DevMsg("Queued event raise\n");
#endif
                    }
#if DEBUG
                    else
                    {
                        this.DevMsg("ConvarChanged: Can't find var with id={0}\n", nativeID);
                    }
#endif
                }
                finally
                {
                    this._lckConvars.ReleaseReaderLock();
                }
#if DEBUG
            }
            catch (Exception ex)
            {
                this.Warning(ex);
            }
            finally
            {
                this.DevMsg("ConvarChanged({0}) Exit\n", nativeID);
            }
#endif
        }

        private void ConvarChangedRaise(ConVarEntry entry)
        {
#if DEBUG
            this.DevMsg("Threaded ConvarChangedRaise enter...\n");
            try
            {
#endif
                //ConVarEntry entry = (ConVarEntry)state;
                entry.Var.RaiseValueChanged();
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

        internal bool UnregisterConvar(ClsPluginBase plugin, ClsConvarMain var)
        {
            Check.NonNull("var", var);

            this._lckConvars.AcquireReaderLock(Timeout.Infinite);
            try
            {
                if (this._convarsList.ContainsKey(var.NativeID))
                {
                    LockCookie cookie = this._lckConvars.UpgradeToWriterLock(Timeout.Infinite);
                    try
                    {
                        if (this.InterThreadCall<bool, UInt64>(this.UnregisterConvar_Call, var.NativeID))
                        {
                            this._convarsList.Remove(var.NativeID);
                            return true;
                        }
                    }
                    finally
                    {
                        this._lckConvars.DowngradeFromWriterLock(ref cookie);
                    }
                }
            }
            finally
            {
                this._lckConvars.ReleaseReaderLock();
            }

            //lock (this._convars)
            //{
            //    if (this._convars.ContainsKey(var.NativeID))
            //    {
            //        if (this.InterThreadCall<bool, UInt64>(this.UnregisterConvar_Call, var.NativeID))
            //        {
            //            this._convars.Remove(var.NativeID);
            //            return true;
            //        }
            //    }
            //}
            return false;
        }

        private bool UnregisterConvar_Call(UInt64 nativeID)
        {
            return NativeMethods.Mono_UnregisterConvar(nativeID);
        }
    }
}
