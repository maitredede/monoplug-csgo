using System;
using System.Threading;

namespace MonoPlug
{
    /// <summary>
    /// Sample console clock
    /// </summary>
    public sealed class ConClock : ClsPluginBase
    {
        /// <summary>
        /// Get plugin name
        /// </summary>
        public override string Name { get { return "ConClock"; } }
        /// <summary>
        /// Get plugin description
        /// </summary>
        public override string Description
        {
            get { return "Write current time to console each seconds, and update a convar with time value"; }
        }

        private Timer _t = null;

        private ClsConVar _theTime = null;
        private ClsConVar _enabled = null;

        /// <summary>
        /// Load the plugin
        /// </summary>
        protected override void Load()
        {
            //this.DevMsg("ConClock:{0}\n", "A");
            this._theTime = this.ConItems.RegisterConvar("clr_sample_thetime", "Sample convar containing the time", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_PRINTABLEONLY, DateTime.MinValue.ToLongTimeString());
            //this.DevMsg("ConClock:{0}\n", "B");
            this._enabled = this.ConItems.RegisterConvar("clr_sample_thetime_enabled", "Enable or disable the ConClock plugin", FCVAR.FCVAR_NONE, "0");
            //this.DevMsg("ConClock:{0}\n", "C");
            this._enabled.ValueChanged += this._enabled_ValueChanged;
            //this.DevMsg("ConClock:{0}\n", "D");
            this._theTime.ValueChanged += this._theTime_ValueChanged;
        }

        private void _theTime_ValueChanged(object sender, EventArgs e)
        {
            this.Message.Msg("ConClock : {0}\n", this._theTime.ValueString);
        }

        private void _enabled_ValueChanged(object sender, EventArgs e)
        {
#if DEBUG
            this.Message.DevMsg("ConClock::EnabledValueChanged enter...\n");
            try
            {
#endif
                bool value = this._enabled.ValueBoolean;
                //this.DevMsg("ConClock : Value is {0}\n", value);
                if (value)
                {
                    if (this._t == null)
                    {
                        this._t = new Timer(this.Tick, null, 1000, 1000);
                    }
                }
                else
                {
                    if (this._t != null)
                    {
                        this._t.Dispose();
                        this._t = null;
                    }
                }
#if DEBUG
            }
            finally
            {
                this.Message.DevMsg("ConClock::EnabledValueChanged Exit...\n");
            }
#endif
        }

        /// <summary>
        /// Unload the plugin
        /// </summary>
        protected override void Unload()
        {
            this._enabled.ValueBoolean = false;
            this._enabled.ValueChanged -= this._enabled_ValueChanged;
            this._theTime.ValueChanged -= this._theTime_ValueChanged;

            this.ConItems.UnregisterConvar(this._enabled);
            this._enabled = null;
            this.ConItems.UnregisterConvar(this._theTime);
            this._theTime = null;
        }

        private void Tick(object state)
        {
#if DEBUG
            this.Message.DevMsg("ConClock : Tick (enter)\n");
            try
            {
#endif
                this._theTime.ValueString = DateTime.Now.ToLongTimeString();
#if DEBUG
            }
            finally
            {
                this.Message.DevMsg("ConClock : Tick (exit)\n");
            }
#endif
        }
    }
}