using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
#if !DEBUG
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    public abstract class PluginBase : IPlugin
    {
        public abstract void Load();
        public abstract void Unload();

        private IEngine m_engine;

        void IPlugin.Init(IEngine engine)
        {
            this.m_engine = engine;
        }

        protected IEngine Engine { get { return this.m_engine; } }
    }
}
