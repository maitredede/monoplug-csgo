using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// A Convar used by managed code
    /// </summary>
    //[Obsolete("Temporary, for rewriting...", true)]
    public class ClsConvar : MarshalByRefObject /* ClsConCommandBase */
    {
        internal readonly ClsConvarMain _remoteMain;

        internal ClsConvar(ClsConvarMain main)
        {
            this._remoteMain = main;
        }

        internal ClsConvarMain ConvarMain { get { return this._remoteMain; } }

        //internal readonly ClsMain _main;

        //internal ClsConvar(ClsMain main, UInt64 nativeId, string name, string description, FCVAR flags)
        //    : base(nativeId, name, description, flags)
        //{
        //    this._main = main;
        //}

        /// <summary>
        /// Raised when the value has changed
        /// </summary>
        public event EventHandler ValueChanged;

        internal void RaiseValueChanged()
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Convar value as string
        /// </summary>
        public string ValueString
        {
            get
            {
                return this._remoteMain.GetString();
            }
            set
            {
                this._remoteMain.SetValue(value);
            }
        }

        /// <summary>
        /// Convar value as boolean
        /// </summary>
        public bool ValueBoolean
        {
            get
            {
                return this._remoteMain.GetBoolean();
            }
            set
            {
                this._remoteMain.SetValue(value);
            }
        }
    }
}
