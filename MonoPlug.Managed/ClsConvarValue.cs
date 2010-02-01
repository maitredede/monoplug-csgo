using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsConvarValue : MarshalByRefObject, IConVarValue
    {
        private readonly ClsMain _main;

        public ClsConvarValue(ClsMain main)
        {
            this._main = main;
        }


        string IConVarValue.GetValueString(UInt64 nativeID)
        {
            return this._main.InterThreadCall<string, UInt64>(this.ConVar_GetString, nativeID);
        }

        private string ConVar_GetString(UInt64 nativeID)
        {
            return NativeMethods.Mono_Convar_GetString(nativeID);
        }

        void IConVarValue.SetValueString(ulong nativeID, string value)
        {
            this._main.InterThreadCall<object, CTuple<UInt64, string>>(this.ConVar_SetString, new CTuple<UInt64, string>(nativeID, value));
        }

        private object ConVar_SetString(CTuple<UInt64, string> data)
        {
            NativeMethods.Mono_Convar_SetString(data.Item1, data.Item2);
            return null;
        }

        bool IConVarValue.GetValueBool(ulong nativeID)
        {
            return this._main.InterThreadCall<bool, UInt64>(this.ConVar_GetBool, nativeID);
        }

        private bool ConVar_GetBool(UInt64 nativeID)
        {
            return NativeMethods.Mono_Convar_GetBoolean(nativeID);
        }

        void IConVarValue.SetValueBool(ulong nativeID, bool value)
        {
            this._main.InterThreadCall<object, CTuple<UInt64, bool>>(this.ConVar_SetBool, new CTuple<UInt64, bool>(nativeID, value));
        }

        private object ConVar_SetBool(CTuple<UInt64, bool> data)
        {
            NativeMethods.Mono_Convar_SetBoolean(data.Item1, data.Item2);
            return null;
        }
    }
}
