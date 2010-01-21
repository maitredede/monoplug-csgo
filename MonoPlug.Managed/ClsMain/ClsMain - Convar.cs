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

            lock (this._convars)
            {
                UInt64 nativeID = this.InterThreadCall<UInt64, ConvarRegisterData>(this.RegisterConvar_Call, data);
                if (nativeID > 0)
                {
                    //ClsConvarMain var = new ClsConvarMain(this, nativeID, name, help, flags);
                    ClsConvarMain var = new ClsConvarMain(this, nativeID);
                    ConVarEntry entry = new ConVarEntry(plugin, var, data);
                    this._convars.Add(nativeID, entry);
                    return var;
                }
                else
                {
                    this.Msg("Can't register var {0} for plugin {1}\n", name, plugin ?? (object)"<main>");
                    return null;
                }
            }
        }

        private UInt64 RegisterConvar_Call(ConvarRegisterData data)
        {
            return NativeMethods.Mono_RegisterConvar(data.Name, data.Help, (int)data.Flags, data.DefaultValue);
        }

        internal void ConvarChanged(UInt64 nativeID)
        {
#if DEBUG
            NativeMethods.Mono_DevMsg(string.Format("ConvarChanged({0}) A", nativeID));
#endif
            lock (this._convars)
            {
                NativeMethods.Mono_DevMsg(string.Format("ConvarChanged({0}) B", nativeID));
                if (this._convars.ContainsKey(nativeID))
                {
                    NativeMethods.Mono_DevMsg(string.Format("ConvarChanged({0}) C", nativeID));
                    ConVarEntry entry = this._convars[nativeID];
                    NativeMethods.Mono_DevMsg(string.Format("ConvarChanged({0}) D", nativeID));
                    ThreadPool.QueueUserWorkItem(this.ConvarChangedRaise, entry);
                    NativeMethods.Mono_DevMsg(string.Format("ConvarChanged({0}) E", nativeID));
                }
            }

            NativeMethods.Mono_DevMsg(string.Format("ConvarChanged({0}) F", nativeID));
        }

        private void ConvarChangedRaise(object state)
        {
            ConVarEntry entry = (ConVarEntry)state;
            entry.Var.RaiseValueChanged();
        }

        internal bool UnregisterConvar(ClsPluginBase plugin, ClsConvarMain var)
        {
            if (var == null) throw new ArgumentNullException("var");

            lock (this._convars)
            {
                if (this._convars.ContainsKey(var.NativeID))
                {
                    if (this.InterThreadCall<bool, UInt64>(this.UnregisterConvar_Call, var.NativeID))
                    {
                        this._convars.Remove(var.NativeID);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool UnregisterConvar_Call(UInt64 nativeID)
        {
            return NativeMethods.Mono_UnregisterConvar(nativeID);
        }
    }
}
