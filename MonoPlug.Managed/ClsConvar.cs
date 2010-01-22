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
        internal readonly IMessage _msg;

        internal ClsConvar(ClsConvarMain main, IMessage msg)
        {
            this._remoteMain = main;
            this._msg = msg;

#if DEBUG
            this._msg.DevMsg("ClsConvar::new() in [{0}]\n", AppDomain.CurrentDomain.FriendlyName);
#endif
        }

        internal ClsConvarMain ConvarMain { get { return this._remoteMain; } }

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
