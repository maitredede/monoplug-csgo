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
        private readonly Dictionary<int, ManagedCommand> m_commands;

        private static int s_commandId = 0;

        internal Engine_Win32(PluginManager manager)
            : base(manager)
        {
            this.m_commands = new Dictionary<int, ManagedCommand>();
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

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void CommandExecuteDelegateWrapper(int argc,
                                                            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
                                                        string[] args);

        public Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            if (this.m_cb_RegisterCommand == null)
                return Task.FromResult(-1);

            int id = Interlocked.Increment(ref s_commandId);
            ManagedCommand cmd = new ManagedCommand
            {
                Id = id,
                Name = command,
                Description = description,
                Flags = flags,
                Callback = callback,
            };

            byte[] cmdUTF8 = this.m_enc.GetBytes(command);
            byte[] descUTF8 = this.m_enc.GetBytes(description);
            int iFlags = (int)flags;
            //IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
            //return this.m_fact.StartNew(() => this.m_cb_RegisterCommand(cmdUTF8, descUTF8, iFlags, callbackPtr));
            //GCHandle gch = GCHandle.Alloc(callback);
            //IntPtr ip = Marshal.GetFunctionPointerForDelegate(callback);

            CommandExecuteDelegateWrapper wrapper = (argc, args) =>
            {
                callback(args);
            };
            //

            return this.m_fact.StartNew(() =>
            {
                ////IntPtr ip = Marshal.GetFunctionPointerForDelegate(callback);
                ////GCHandle gch = GCHandle.Alloc(wrapper, GCHandleType.Pinned);
                //GCHandle gch = GCHandle.Alloc(wrapper);
                //IntPtr ip = gch.AddrOfPinnedObject();
                //int id = this.m_cb_RegisterCommand(cmdUTF8, descUTF8, iFlags, ip);
                ////if (id < 0)
                ////{
                ////    gch.Free();
                ////}
                ////else
                ////{
                ////    lock (this.m_handles)
                ////    {
                ////        this.m_handles.Add(id, gch);
                ////    }
                ////}
                //return id;
                bool succes = this.m_cb_RegisterCommand(cmdUTF8, descUTF8, iFlags, id);
                if (succes)
                {
                    lock (this.m_commands)
                    {
                        this.m_commands.Add(id, cmd);
                    }
                    return id;
                }
                else
                {
                    return -1;
                }
            });
        }

        public Task UnregisterCommand(int id)
        {
            lock (this.m_commands)
            {
                if (this.m_commands.ContainsKey(id))
                {
                    //this.m_commands[id].Free();
                    this.m_commands.Remove(id);
                }
            }
            //TODO : Unregister Command
            return Task.FromResult(id);
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
