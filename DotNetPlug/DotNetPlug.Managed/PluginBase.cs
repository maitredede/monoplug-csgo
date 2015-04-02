using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Base plugin class
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    public abstract class PluginBase : IPlugin
    {
        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public abstract Task Load();
        /// <summary>
        /// Unloads this instance.
        /// </summary>
        /// <returns></returns>
        public abstract Task Unload();

        private IEngine m_engine;

        void IPlugin.Init(IEngine engine)
        {
            this.m_engine = engine;
        }

        /// <summary>
        /// Source engine interface
        /// </summary>
        protected IEngine Engine { get { return this.m_engine; } }
    }
}
