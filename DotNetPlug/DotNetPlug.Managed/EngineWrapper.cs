using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class EngineWrapper : IEngine
    {
        private readonly PluginManager m_manager;
        private readonly TaskFactory m_fact;

        internal EngineWrapper(PluginManager manager)
        {
            this.m_manager = manager;
            this.m_fact = new TaskFactory(this.m_manager.TaskScheduler);
        }


        public Task<string> ExecuteCommand(string command)
        {
            if (this.m_cb_ExecuteCommand == null)
                return Task.FromResult<string>(null);

            throw new NotImplementedException();
        }

        public Task Log(string msg)
        {
            if (this.m_cb_Log == null)
                return Task.FromResult(string.Empty);

            return this.m_fact.StartNew(() => this.m_cb_Log(msg));
            ////return Task.Run(() => this.m_cb_Log(msg));
            //this.m_manager.SynchronizationContext.Post((o) =>
            //{
            //    this.m_cb_Log(msg);
            //}, null);
            //return Task.FromResult(string.Empty);
        }

        internal LogDelegate m_cb_Log;
        internal ExecuteCommandDelegate m_cb_ExecuteCommand;
    }
}
