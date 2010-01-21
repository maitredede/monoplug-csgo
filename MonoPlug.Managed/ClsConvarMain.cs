using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsConvarMain : MarshalByRefObject
    {
        private readonly ClsMain _main;
        private readonly UInt64 _nativeId;
        //private readonly string _name;
        //private readonly string _help;
        //private readonly FCVAR _flags;

        internal UInt64 NativeID { get { return this._nativeId; } }

        private ClsConvar _remote = null;

        //internal ClsConvarMain(ClsMain main, UInt64 nativeId, string name, string help, FCVAR flags)
        internal ClsConvarMain(ClsMain main, UInt64 nativeId)
        {
            this._main = main;
            this._nativeId = nativeId;
            //this._name = name;
            //this._help = help;
            //this._flags = flags;
        }

        internal void SetRemoteVar(ClsConvar remote)
        {
            Check.NonNull("remote", remote);

            this._remote = remote;
        }

        internal void RaiseValueChanged()
        {
            if (this._remote != null)
            {
                this._remote.RaiseValueChanged();
            }
        }

        /// <summary>
        /// Get the Convar value as string
        /// </summary>
        /// <returns>Convar value as string</returns>
        internal string GetString()
        {
            return this._main.InterThreadCall<string, object>(this.GetStringCall, null);
        }

        private string GetStringCall(object state)
        {
            return NativeMethods.Mono_Convar_GetString(this._nativeId);
        }

        /// <summary>
        /// Set Convar value as string
        /// </summary>
        /// <param name="value">Value</param>
        internal void SetValue(string value)
        {
            this._main.InterThreadCall<object, string>(this.SetStringCall, value);
        }

        private object SetStringCall(string value)
        {
            NativeMethods.Mono_Convar_SetString(this._nativeId, value);
            return null;
        }

        internal bool GetBoolean()
        {
            return this._main.InterThreadCall<bool, object>(this.GetBooleanCall, null);
        }

        private bool GetBooleanCall(object state)
        {
            return NativeMethods.Mono_Convar_GetBoolean(this._nativeId);
        }

        internal void SetValue(bool value)
        {
            this._main.InterThreadCall<object, bool>(this.SetBooleanCall, value);
        }

        private object SetBooleanCall(bool value)
        {
            NativeMethods.Mono_Convar_SetBoolean(this._nativeId, value);
            return null;
        }

    }
}
