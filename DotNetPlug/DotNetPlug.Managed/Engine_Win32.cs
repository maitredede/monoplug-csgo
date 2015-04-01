using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class Engine_Win32 : EngineWrapperBase, IEngine
    {
        private readonly Dictionary<int, GCHandle> m_handles;

        internal Engine_Win32(PluginManager manager)
            : base(manager)
        {
            this.m_handles = new Dictionary<int, GCHandle>();
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

                byte[] ret;
                int length;
                this.m_cb_ExecuteCommand(commandUTF8, out ret, out length);
                return this.m_enc.GetString(ret);
            });
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CommandExecuteDelegateWrapper(int argc,
                                                            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
                                                        string[] args);

        public Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            if (this.m_cb_RegisterCommand == null)
                return Task.FromResult(-1);

            byte[] cmdUTF8 = this.m_enc.GetBytes(command);
            byte[] descUTF8 = this.m_enc.GetBytes(description);
            int iFlags = (int)flags;
            //IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
            //return this.m_fact.StartNew(() => this.m_cb_RegisterCommand(cmdUTF8, descUTF8, iFlags, callbackPtr));
            //GCHandle gch = GCHandle.Alloc(callback);
            //IntPtr ip = Marshal.GetFunctionPointerForDelegate(callback);

            CommandExecuteDelegateWrapper wrapper = (argc, args) => callback(args);
            IntPtr ip = Marshal.GetFunctionPointerForDelegate(callback);

            return this.m_fact.StartNew(() =>
            {
                int id = this.m_cb_RegisterCommand(cmdUTF8, descUTF8, iFlags, ip);
                //if (id < 0)
                //{
                //    gch.Free();
                //}
                //else
                //{
                //    lock (this.m_handles)
                //    {
                //        this.m_handles.Add(id, gch);
                //    }
                //}
                return id;
            });
        }

        public Task UnregisterCommand(int id)
        {
            lock (this.m_handles)
            {
                if (this.m_handles.ContainsKey(id))
                {
                    this.m_handles[id].Free();
                    this.m_handles.Remove(id);
                }
            }
            //TODO : Unregister Command
            return Task.FromResult(id);
        }
    }
}
