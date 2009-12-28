using System;
using System.Threading;

namespace MonoPlug
{
    public class ConClock : ClsPluginBase
    {
        public override string Name { get { return "Console clock"; } }

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