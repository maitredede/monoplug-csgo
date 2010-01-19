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
            this._theTime = this.RegisterConvar("clr_sample_thetime", "Sample convar containing the time", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_PRINTABLEONLY, DateTime.MinValue.ToLongTimeString());
            this._enabled = this.RegisterConvar("clr_sample_thetime_enabled", "Enable or disable the ConClock plugin", FCVAR.FCVAR_NONE, "0");
            this._enabled.ValueChanged += this._enabled_ValueChanged;
        }

        void _enabled_ValueChanged(object sender, EventArgs e)
        {
            if (this._enabled.GetBoolean())
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
        }

        protected override void Unload()
        {
            this._enabled.SetValue(false);
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
            this._theTime.SetValue(s);
        }
    }
}