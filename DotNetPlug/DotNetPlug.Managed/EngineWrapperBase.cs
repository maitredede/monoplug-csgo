using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal abstract class EngineWrapperBase
    {
        protected readonly PluginManager m_manager;
        protected readonly TaskFactory m_fact;
        protected readonly Encoding m_enc;

        internal EngineWrapperBase(PluginManager manager)
        {
            this.m_manager = manager;
            this.m_fact = new TaskFactory(this.m_manager.TaskScheduler);
            this.m_enc = Encoding.UTF8;
        }
    }
}
