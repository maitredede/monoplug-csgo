using System;
using System.Threading;

namespace MonoPlug
{
    public sealed class ConClock : ClsPluginBase
    {
        public override string Name { get { return "ConClock"; } }
        public override string Description
        {
            get { return "Write current time to console each seconds, and update a convar with value"; }
        }

        private Timer _t;

        private ClsConvar _theTime;
        private ClsConvar _enabled;

        public ConClock()
        {
        }

        protected override void Load()
        {
            this.DevMsg("ConClock:{0}\n", "A");
            this._theTime = this.RegisterConvar("clr_sample_thetime", "Sample convar containing the time", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_PRINTABLEONLY, DateTime.MinValue.ToLongTimeString());
            this.DevMsg("ConClock:{0}\n", "B");
            this._enabled = this.RegisterConvar("clr_sample_thetime_enabled", "Enable or disable the ConClock plugin", FCVAR.FCVAR_NONE, "0");
            this.DevMsg("ConClock:{0}\n", "C");
            this._enabled.ValueChanged += this._enabled_ValueChanged;
            this.DevMsg("ConClock:{0}\n", "D");
        }

        private void _enabled_ValueChanged(object sender, EventArgs e)
        {
#if DEBUG
            this.DevMsg("ConClock::EnabledValueChanged enter...\n");
            try
            {
#endif
                if (this._enabled.ValueBoolean)
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
                this.DevMsg("ConClock::EnabledValueChanged Exit...\n");
            }
#endif
        }

        protected override void Unload()
        {
            this._enabled.ValueBoolean = false;
            this._enabled.ValueChanged -= this._enabled_ValueChanged;
            this.UnregisterConvar(this._enabled);
            this._enabled = null;
            this.UnregisterConvar(this._theTime);
            this._theTime = null;
        }

        private void Tick(object state)
        {
            string s = DateTime.Now.ToLongTimeString();
            this.Msg(s + "\n");
            this._theTime.ValueString = s;
        }
    }
}