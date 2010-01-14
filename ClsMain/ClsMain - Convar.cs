using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal ClsConvar RegisterConvar(ClsPluginBase plugin, string name, string help, FCVAR flags, string defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(help)) throw new ArgumentNullException("description");
            ValidateFlags(flags, "flags");

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
                    ClsConvar var = new ClsConvar(this, nativeID, name, help, flags);
                    ConVarEntry entry = new ConVarEntry(plugin, var, data);
                    this._convars.Add(nativeID, entry);
                    return var;
                }
                else
                {
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
            Msg("ClsMain::_ConVarStringChanged({0})", nativeID);
#endif
            lock (this._convars)
            {
                if (this._convars.ContainsKey(nativeID))
                {
                    ConVarEntry entry = this._convars[nativeID];
                    ThreadPool.QueueUserWorkItem(this.ConvarChangedRaise, entry);
                }
            }
        }

        private void ConvarChangedRaise(object state)
        {
            ConVarEntry entry = (ConVarEntry)state;
            entry.Var.RaiseValueChanged();
        }

        internal bool UnregisterConvar(ClsPluginBase plugin, ClsConvar var)
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
        //        [Obsolete("Rewriting", true)]
        //        internal ClsConVarStrings RegisterConVarString(ClsPluginBase plugin, string name, string description, FCVAR flags, string defaultValue)
        //        {
        //            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
        //            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
        //            ValidateFlags(flags, "flags");

        //            lock (this._ConVarString)
        //            {
        //                //Check if var exists
        //                if (this.VarStringExists(name)) return null;

        //                //Register native var
        //                UInt64 nativeId = 0;
        //                this.InterThreadCall(() =>
        //                {
        //                    nativeId = NativeMethods.Mono_RegisterConvarString(name, description, (int)flags, defaultValue);
        //                });

        //                Msg("M: RegisterConVarString '{0}' NativeId is {1}\n", name, nativeId);
        //                if (nativeId > 0)
        //                {
        //                    ConVarEntry entry = new ConVarEntry(plugin,var,);
        //                    entry.Plugin = plugin;
        //                    entry.Var = new ClsConVarStrings(this, nativeId, name, description, flags);
        //                    this._ConVarString.Add(nativeId, entry);
        //                    return entry.Var;
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //        }

        //        [Obsolete("Rewriting", true)]
        //        private bool VarStringExists(string name)
        //        {
        //            foreach (UInt64 key in this._ConVarString.Keys)
        //            {
        //                if (this._ConVarString[key].Var.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    return true;
        //                }
        //            }
        //            return false;
        //        }

        //        /// <summary>
        //        /// Callback when an internal convar value has changed;
        //        /// </summary>
        //        /// <param name="nativeId"></param>
        //        [Obsolete("Rewriting", true)]
        //        internal void _ConVarStringChanged(UInt64 nativeId)
        //        {
        //#if DEBUG
        //            Msg("ClsMain::_ConVarStringChanged({0})", nativeId);
        //#endif
        //            if (this._ConVarString.ContainsKey(nativeId))
        //            {
        //                ThreadPool.QueueUserWorkItem((object o) =>
        //                {
        //                    this._ConVarString[nativeId].Var.RaiseValueChanged();
        //                });
        //            }
        //        }

        //        [Obsolete("Rewriting", true)]
        //        internal void UnregisterConVarString(ClsPluginBase plugin, ClsConVarStrings convar)
        //        {
        //            if (convar == null) throw new ArgumentNullException("convar");

        //            lock (this._ConVarString)
        //            {
        //                if (this._ConVarString.ContainsKey(convar.NativeID))
        //                {
        //                    this.InterThreadCall(() =>
        //                    {
        //                        NativeMethods.Mono_UnregisterConVarString(convar.NativeID);
        //                    });
        //                    this._ConVarString.Remove(convar.NativeID);
        //                }
        //            }
        //        }

        //        [Obsolete("Rewriting", true)]
        //        internal string GetConvarStringValue(string name)
        //        {
        //            string value = null;
        //            this.InterThreadCall(() =>
        //                {
        //                    value = NativeMethods.Mono_Convar_GetValue_String(name);
        //                });
        //            return value;
        //        }
    }
}
