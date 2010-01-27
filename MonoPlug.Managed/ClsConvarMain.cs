using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    [Obsolete("", true)]
    internal sealed class ClsConvarMain : MarshalByRefObject
    {
        private readonly ClsMain m_main;
        private readonly string m_name;
        private readonly string m_help;
        private readonly FCVAR m_flags;
        private readonly string m_defaultValue;

        private UInt64 m_nativeId;
        private ClsConvar m_remote = null;

        internal UInt64 NativeID { get { return this.m_nativeId; } }
        internal IMessage Msg { get { return this.m_main; } }
        internal string Name { get { return this.m_name; } }
        internal string Help { get { return this.m_help; } }
        internal FCVAR Flags { get { return this.m_flags; } }
        internal string DefaultValue { get { return this.m_defaultValue; } }

        internal ClsConvarMain(ClsMain main, string name, string help, FCVAR flags, string defaultValue)
        {
#if DEBUG
            main.DevMsg("ClsConvarMain::new() in [{0}]\n", AppDomain.CurrentDomain.FriendlyName);
#endif
            this.m_main = main;
            this.m_name = name;
            this.m_help = help;
            this.m_flags = flags;
            this.m_defaultValue = defaultValue;
        }

        internal void Init(UInt64 nativeId)
        {
            Check.InRange("nativeId", nativeId, 0, UInt64.MaxValue);
            this.m_nativeId = nativeId;
        }

        internal void Init(ClsConvar remote)
        {
            Check.NonNull("remote", remote);
            this.m_remote = remote;
        }

        internal void RaiseValueChanged()
        {
#if DEBUG
            this.m_main.DevMsg("Entering {0}.RaiseValueChanged()\n", this.GetType().Name);
            try
            {
#endif
                if (this.m_remote != null)
                {
                    this.m_remote.RaiseValueChanged();
                }
#if DEBUG
            }
            finally
            {
                this.m_main.DevMsg("Exiting {0}.RaiseValueChanged()\n", this.GetType().Name);
            }
#endif
        }

        /// <summary>
        /// Get the Convar value as string
        /// </summary>
        /// <returns>Convar value as string</returns>
        internal string GetString()
        {
            return this.m_main.InterThreadCall<string, object>(this.GetStringCall, null);
        }

        private string GetStringCall(object state)
        {
            return NativeMethods.Mono_Convar_GetString(this.m_nativeId);
        }

        /// <summary>
        /// Set Convar value as string
        /// </summary>
        /// <param name="value">Value</param>
        internal void SetValue(string value)
        {
            this.m_main.InterThreadCall<object, string>(this.SetStringCall, value);
        }

        private object SetStringCall(string value)
        {
            NativeMethods.Mono_Convar_SetString(this.m_nativeId, value);
            return null;
        }

        internal bool GetBoolean()
        {
#if DEBUG
            this.m_main.DevMsg("Entering {0}.GetBoolean()\n", this.GetType().Name);
            try
            {
#endif
                return this.m_main.InterThreadCall<bool, object>(this.GetBooleanCall, null);
#if DEBUG
            }
            finally
            {
                this.m_main.DevMsg("Exiting {0}.GetBoolean()\n", this.GetType().Name);
            }
#endif
        }

        private bool GetBooleanCall(object state)
        {
            return NativeMethods.Mono_Convar_GetBoolean(this.m_nativeId);
        }

        internal void SetValue(bool value)
        {
            this.m_main.InterThreadCall<object, bool>(this.SetBooleanCall, value);
        }

        private object SetBooleanCall(bool value)
        {
            NativeMethods.Mono_Convar_SetBoolean(this.m_nativeId, value);
            return null;
        }

    }
}
