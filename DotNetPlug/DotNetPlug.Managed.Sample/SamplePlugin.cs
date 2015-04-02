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
        private readonly List<int> m_commands = new List<int>();

        public override async Task Load()
        {
            //Start a timer for threaded demo
            this.m_timer = new Timer(this.ClockTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));

            //Add a sample command for managed code
            int cmd = await this.Engine.RegisterCommand("managed_status", "Managed status", FCVar.ServerCanExecute, this.Managed_Status);
            this.m_commands.Add(cmd);
        }

        public override async Task Unload()
        {
            if (this.m_timer != null)
            {
                this.m_timer.Dispose();
            }
            if (this.m_commands.Count > 0)
            {
                foreach (int id in this.m_commands)
                {
                    await this.Engine.UnregisterCommand(id);
                }
                this.m_commands.Clear();
            }
        }

        private async void ClockTick(object state)
        {
            await this.Engine.Log("Time from managed clock is : {0}", DateTime.Now.ToLongTimeString());

            IPlayer[] players = await this.Engine.GetPlayers();
            await this.Engine.Log("There is {0} players connected", players.Length);
        }

        private async void Managed_Status(string[] param)
        {
            string result = await this.Engine.ExecuteCommand("status");
            await this.Engine.Log(result);
        }
    }
}
