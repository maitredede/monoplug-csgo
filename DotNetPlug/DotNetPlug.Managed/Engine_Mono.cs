using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class Engine_Mono : EngineWrapperBase
    {
        internal Engine_Mono(PluginManager manager)
            : base(manager)
        {
        }

        public override Task Log(string msg)
        {
            return this.m_fact.StartNew(() => PluginManagerMono.Log(msg));
        }

        public override Task<string> ExecuteCommand(string command)
        {
            return this.m_fact.StartNew(() =>
            {
                string ret;
                int len;
                PluginManagerMono.ExecuteCommand(command, out ret, out len);
                return ret;
            });
        }

        public override Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            return this.m_fact.StartNew(() => PluginManagerMono.RegisterCommand(command, description, flags, callback));
        }
    }
}
