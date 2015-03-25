using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class ClockTick : PluginBase, IPlugin
    {
        private Timer m_timer;

        public override void Load()
        {
            this.m_timer = new Timer(this.Tick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
        }

        private void Tick(object state)
        {
            this.Engine.Log(string.Format("Time from managed clock is : {0}", DateTime.Now.ToLongTimeString())).Wait();
        }

        public override void Unload()
        {
            this.m_timer.Dispose();
        }
    }
}
