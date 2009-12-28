using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        private Dictionary<UInt64, ConVarEntry> _varString;

        internal ClsConVarStrings RegisterConVarString(ClsPluginBase plugin, string name, string description, FCVAR flags, string defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("description");
            ValidateFlags(flags, "flags");

            lock (this._varString)
            {
                //Check if var exists
                if (this.VarStringExists(name)) return null;

                //Register native var
                UInt64 nativeId = Mono_RegisterConVarString(name, description, (int)flags, defaultValue);

                if (nativeId > 0)
                {
                    ConVarEntry entry = new ConVarEntry();
                    entry.Plugin = plugin;
                    entry.Var = new ClsConVarStrings(nativeId, name, description, flags);
                    this._varString.Add(nativeId, entry);
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
            foreach (UInt64 key in this._varString.Keys)
            {
                if (this._varString[key].Var.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
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
#if DEBUG
            Mono_Msg(string.Format("M:_ConVarStringChanged({0})", nativeId));
#endif
            if (this._varString.ContainsKey(nativeId))
            {
                this._varString[nativeId].Var.RaiseValueChanged();
            }
        }
    }
}
