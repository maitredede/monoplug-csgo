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
        private int m_loadType;
        private readonly PluginManager m_manager;

        internal CorePlugin(PluginManager manager)
        {
            this.m_manager = manager;
        }

        public override Task Load()
        {
            throw new InvalidOperationException();
        }

        internal void LoadSync()
        {
            this.m_loadAssembly = this.m_manager.EngineWrapper.RegisterCommandInternal("load_assembly", "Load assembly plugin", FCVar.ServerCanExecute, PluginManager.Instance.LoadAssembly);
            this.m_loadType = this.m_manager.EngineWrapper.RegisterCommandInternal("load_type", "Load type plugin", FCVar.ServerCanExecute, PluginManager.Instance.LoadType);
            this.Engine.ExecuteCommand("log on");
        }

        public override Task Unload()
        {
            throw new InvalidOperationException();
        }

        internal void UnloadSync()
        {
            this.m_manager.EngineWrapper.UnregisterCommandSync(this.m_loadAssembly);
        }
    }
}
