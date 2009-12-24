using System;
using System.Threading;

namespace MonoPlug
{
	
	
	public class ConClock : ClsPluginBase
	{
		public override string Name{get{return "Console clock";}}
		
		private Timer _t;
		
		public ConClock()
		{
		}
		
		protected override void Load ()
		{
			this._t=new Timer(this.Tick, null, 1000, 1000);
		}

		protected override void Unload ()
		{
			this._t.Dispose();
			this._t=null;
		}

		private void Tick(object state)
		{
			Msg(DateTime.Now.ToLongTimeString());
		}
	}
}
