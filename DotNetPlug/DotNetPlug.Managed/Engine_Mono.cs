using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class Engine_Mono : EngineWrapperBase, IEngine
    {
        internal Engine_Mono(PluginManager manager)
            : base(manager)
        {
        }

        public Task Log(string msg)
        {
            return this.m_fact.StartNew(() => PluginManagerMono.Log(msg));
        }

        public Task<string> ExecuteCommand(string command)
        {
            return this.m_fact.StartNew(() =>
            {
                string ret;
                int len;
                PluginManagerMono.ExecuteCommand(command, out ret, out len);
                return ret;
            });
        }

        public Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            return this.m_fact.StartNew(() => PluginManagerMono.RegisterCommand(command, description, flags, callback));
        }

        public Task UnregisterCommand(int id)
        {
            //TODO : Unregister Command
            return Task.FromResult(id);
        }

        void IEngine.RaiseCommand(int id, int argc, string[] argv)
        {

        }
    }
}
