using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class CorePlugin : PluginBase
    {
        private int m_loadAssembly;

        public override async Task Load()
        {
            this.m_loadAssembly = await this.Engine.RegisterCommand("load_assembly", "Load assembly plugin", FCVar.ServerCanExecute, PluginManager.Instance.LoadAssembly);
        }

        public override async Task Unload()
        {
            await this.Engine.UnregisterCommand(this.m_loadAssembly);
        }
    }
}
