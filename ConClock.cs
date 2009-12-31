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

        private ClsConVarStrings _theTime;

        public ConClock()
        {
        }

        protected override void Load()
        {
            this._t = new Timer(this.Tick, null, 1000, 1000);
            this._theTime = this.RegisterConVarString("clr_sample_thetime", "Sample convar containing the time", FCVAR.FCVAR_GAMEDLL | FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_PRINTABLEONLY, DateTime.MinValue.ToLongTimeString());
        }

        protected override void Unload()
        {
            this.UnregisterConVarString(this._theTime);
            this._theTime = null;
            this._t.Dispose();
            this._t = null;
        }

        private void Tick(object state)
        {
            string s = DateTime.Now.ToLongTimeString();
            Msg(s + "\n");
            this._theTime.Value = s;
        }
    }
}