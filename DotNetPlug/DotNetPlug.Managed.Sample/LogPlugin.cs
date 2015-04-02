using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.Managed
{
    public sealed class LogPlugin : PluginBase
    {
        private async Task Log(string msg)
        {
            await this.Engine.Log("Log: {0}", msg);
        }

        public override async Task Load()
        {
            await this.Log("Load");
            this.Engine.LevelInit += this.Engine_LevelInit;
        }

        private async void Engine_LevelInit(object sender, LevelInitEventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        public override async Task Unload()
        {
            await this.Log("Unload");
        }
    }
}
