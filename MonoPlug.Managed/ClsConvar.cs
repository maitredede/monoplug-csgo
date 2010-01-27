using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// A Convar used by managed code
    /// </summary>
    [Obsolete("", true)]
    public sealed class ClsConvar : MarshalByRefObject /* ClsConCommandBase */
    {
        internal readonly ClsConvarMain _remoteMain;

        internal ClsConvarMain ConvarMain { get { return this._remoteMain; } }

        internal ClsConvar(ClsConvarMain main)
        {
            this._remoteMain = main;

#if DEBUG
            this._remoteMain.Msg.DevMsg("ClsConvar::new() in [{0}]\n", AppDomain.CurrentDomain.FriendlyName);
#endif
        }


        /// <summary>
        /// Raised when the value has changed
        /// </summary>
        public event EventHandler ValueChanged;

        internal void RaiseValueChanged()
        {
#if DEBUG
            this._remoteMain.Msg.DevMsg("ClsConvar::RaiseValueChanged() enter in [{0}]\n", AppDomain.CurrentDomain.FriendlyName);
            try
            {
#endif
                if (this.ValueChanged != null)
                {
                    this.ValueChanged(this, EventArgs.Empty);
                }
#if DEBUG
            }
            catch (Exception ex)
            {
                this._remoteMain.Msg.Warning(ex);
            }
            finally
            {
                this._remoteMain.Msg.DevMsg("ClsConvar::RaiseValueChanged() Exit \n");
            }
#endif
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
