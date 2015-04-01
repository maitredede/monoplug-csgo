using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class Engine_Win32 : EngineWrapperBase, IEngine
    {
        internal Engine_Win32(PluginManager manager)
            : base(manager)
        {
        }

        internal LogDelegate m_cb_Log;
        internal ExecuteCommandDelegate m_cb_ExecuteCommand;
        internal RegisterCommandDelegate m_cb_RegisterCommand;

        public Task Log(string msg)
        {
            if (this.m_cb_Log == null)
                return Task.FromResult(string.Empty);

            byte[] msgUTF8 = this.m_enc.GetBytes(msg);

            return this.m_fact.StartNew(() => this.m_cb_Log(msgUTF8));
        }

        public Task<string> ExecuteCommand(string command)
        {
            if (this.m_cb_ExecuteCommand == null)
                return Task.FromResult<string>(null);

            if (this.m_cb_ExecuteCommand == null)
                return Task.FromResult(string.Empty);

            byte[] commandUTF8 = this.m_enc.GetBytes(command);
            return this.m_fact.StartNew<string>(() =>
            {
                IntPtr outRet = IntPtr.Zero;
                byte[] ret = null;
                int length = 0;
                this.m_cb_ExecuteCommand(commandUTF8, out outRet, out length);

                ret = new byte[length];
                Marshal.Copy(outRet, ret, 0, length);
                Marshal.FreeCoTaskMem(outRet);

                return this.m_enc.GetString(ret);
            });
        }

        public Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            if (this.m_cb_RegisterCommand == null)
                return Task.FromResult(-1);

            ManagedCommand cmd = base.CreateCommand(command, description, flags, callback);

            byte[] cmdUTF8 = this.m_enc.GetBytes(command);
            byte[] descUTF8 = this.m_enc.GetBytes(description);
            int iFlags = (int)flags;

            return this.m_fact.StartNew(() =>
            {
                bool succes = this.m_cb_RegisterCommand(cmdUTF8, descUTF8, iFlags, cmd.Id);
                if (!succes)
                {
                    base.UnregisterCommand(cmd.Id).Wait();
                    return -1;
                }
                return cmd.Id;
            });
        }

        Task IEngine.UnregisterCommand(int id)
        {
            return base.UnregisterCommand(id);
        }

        void IEngine.RaiseCommand(int id, int argc, string[] argv)
        {
            ManagedCommand cmd;
            if (this.m_commands.TryGetValue(id, out cmd))
            {
                cmd.Callback.BeginInvoke(argv, null, null);
            }
        }
    }
}
