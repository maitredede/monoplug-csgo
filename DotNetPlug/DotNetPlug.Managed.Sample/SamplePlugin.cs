using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class SamplePlugin : PluginBase, IPlugin
    {
        private Timer m_timer;

        public override async Task Load()
        {
            this.m_timer = new Timer(this.ClockTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));

            await this.Engine.RegisterCommand("managed_status", "Managed status", FCVar.ServerCanExecute, this.Managed_Status);
        }

        private void ClockTick(object state)
        {
            this.Engine.Log("Time from managed clock is : {0}", DateTime.Now.ToLongTimeString()).Wait();
        }

        public override Task Unload()
        {
            if (this.m_timer != null)
            {
                this.m_timer.Dispose();
            }
            return Task.FromResult(0);
        }

        public async void Managed_Status(string[] param)
        {
            string result = await this.Engine.ExecuteCommand("status");
            await this.Engine.Log(result);
        }
    }
}
