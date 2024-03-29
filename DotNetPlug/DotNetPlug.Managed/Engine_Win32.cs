﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal sealed class Engine_Win32 : EngineWrapperBase
    {
        internal Engine_Win32(PluginManager manager)
            : base(manager)
        {
        }

        internal LogDelegate m_cb_Log;
        internal ExecuteCommandDelegate m_cb_ExecuteCommand;
        internal RegisterCommandDelegate m_cb_RegisterCommand;
        internal UnregisterCommandDelegate m_cb_UnregisterCommand;
        internal GetPlayersDelegate m_cb_GetPlayers;

        public override Task Log(string msg)
        {
            if (this.m_cb_Log == null)
                return Task.FromResult(string.Empty);

            byte[] msgUTF8 = this.m_enc.GetBytes(msg);

            return this.m_fact.StartNew(() => this.m_cb_Log(msgUTF8));
        }

        public override Task<string> ExecuteCommand(string command)
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

        protected override int RegisterCommandImpl(byte[] cmdUTF8, byte[] descUTF8, int iFlags, int id)
        {
            bool succes = this.m_cb_RegisterCommand(cmdUTF8, descUTF8, iFlags, id);
            if (!succes)
            {
                base.UnregisterCommandDic(id);
                return -1;
            }
            return id;
        }

        //public override Task UnregisterCommand(int id)
        //{
        //    base.UnregisterCommandDic(id);

        //    return this.m_fact.StartNew(() =>
        //    {
        //        this.m_cb_UnregisterCommand(id);
        //    });
        //}

        protected override void UnregisterCommandImpl(int id)
        {
            this.m_cb_UnregisterCommand(id);
        }

        protected override void ShowMOTDInternal(int playerId, byte[] titleUTF8, byte[] msgUTF8, MOTDType type, byte[] cmdUTF8)
        {
            throw new NotImplementedException();
        }

        public override async Task<PlayerData[]> GetPlayers()
        {
            try
            {
                IntPtr ptrData = IntPtr.Zero;
                int nbr = 0;
                await this.m_fact.StartNew(() =>
                {
                    this.m_cb_GetPlayers(out ptrData, out nbr);
                });
                NativePlayerData[] data = new NativePlayerData[nbr];

                MarshalArray(ptrData, data);

                Marshal.FreeCoTaskMem(ptrData);
                return data.Select(p => new PlayerData(p)).ToArray(); ;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void MarshalArray<T>(IntPtr ptr, T[] array)
        {
            Type t = typeof(T);
            for (int i = 0; i < array.Length; i++)
            {
                IntPtr pos = ptr + Marshal.SizeOf(t) * i;
                T item = (T)Marshal.PtrToStructure(pos, t);
                array[i] = item;
            }
        }
    }
}
