using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        private Dictionary<UInt64, ConVarEntry> _ConVarString = null;
        private ClsConVarStrings _clr_mono_version = null;

        internal ClsConVarStrings RegisterConVarString(ClsPluginBase plugin, string name, string description, FCVAR flags, string defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
            ValidateFlags(flags, "flags");

            lock (this._ConVarString)
            {
                //Check if var exists
                if (this.VarStringExists(name)) return null;

                //Register native var
                UInt64 nativeId = Mono_RegisterConVarString(name, description, (int)flags, defaultValue);

                Mono_Msg(string.Format("M: RegisterConVarString '{0}' NativeId is {1}\n", name, nativeId));
                if (nativeId > 0)
                {
                    ConVarEntry entry = new ConVarEntry();
                    entry.Plugin = plugin;
                    entry.Var = new ClsConVarStrings(nativeId, name, description, flags);
                    this._ConVarString.Add(nativeId, entry);
                    return entry.Var;
                }
                else
                {
                    return null;
                }
            }
        }

        private bool VarStringExists(string name)
        {
            foreach (UInt64 key in this._ConVarString.Keys)
            {
                if (this._ConVarString[key].Var.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Callback when an internal convar value has changed;
        /// </summary>
        /// <param name="nativeId"></param>
        internal void _ConVarStringChanged(UInt64 nativeId)
        {
            //#if DEBUG
            //            Mono_Msg(string.Format("M:_ConVarStringChanged({0})", nativeId));
            //#endif
            if (this._ConVarString.ContainsKey(nativeId))
            {
                this._ConVarString[nativeId].Var.RaiseValueChanged();
            }
        }

        internal void UnregisterConVarString(ClsPluginBase plugin, ClsConVarStrings convar)
        {
            if (convar == null) throw new ArgumentNullException("convar");

            lock (this._ConVarString)
            {
                if (this._ConVarString.ContainsKey(convar.NativeID))
                {
                    Mono_UnregisterConVarString(convar.NativeID);
                    this._ConVarString.Remove(convar.NativeID);
                }
            }
        }
    }
}
