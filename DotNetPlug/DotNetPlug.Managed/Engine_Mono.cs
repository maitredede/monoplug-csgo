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

        protected override int RegisterCommandImpl(byte[] cmdUTF8, byte[] descUTF8, int iFlags, int id)
        {
            return PluginManagerMono.RegisterCommand(cmdUTF8, descUTF8, iFlags, id);
        }

        protected override void UnregisterCommandImpl(int id)
        {
            PluginManagerMono.UnregisterCommand(id);
        }

        protected override void ShowMOTDInternal(int playerId, byte[] titleUTF8, byte[] msgUTF8, MOTDType type, byte[] cmdUTF8)
        {
            PluginManagerMono.ShowMOTD(playerId, titleUTF8, msgUTF8, (int)type, cmdUTF8);
        }
    }
}
