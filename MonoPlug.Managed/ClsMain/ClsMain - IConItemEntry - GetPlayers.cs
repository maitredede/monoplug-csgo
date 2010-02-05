using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain : IEngineWrapper
    {
        private readonly Dictionary<int, ClsPlayer> _lstPlayers = new Dictionary<int, ClsPlayer>();
        private readonly ReaderWriterLock _lckPlayers = new ReaderWriterLock();

        ClsPlayer[] IEngineWrapper.GetPlayers()
        {
            List<ClsPlayer> lst = new List<ClsPlayer>();
            this._lckPlayers.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (int id in this._lstPlayers.Keys)
                {
                    lst.Add(this._lstPlayers[id]);
                }
                return lst.ToArray();
            }
            finally
            {
                this._lckPlayers.ReleaseReaderLock();
            }
        }

        private void AddPlayer(ClsPlayer player)
        {
            this._lckPlayers.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (this._lstPlayers.ContainsKey(player.Id))
                {
                    ClsPlayer current = this._lstPlayers[player.Id];
                    foreach (PropertyInfo prop in player.GetType().GetProperties())
                    {
                        if (prop.CanRead && prop.CanWrite)
                        {
                            prop.SetValue(current, prop.GetValue(player, null), null);
                        }
                    }
                }
                else
                {
                    this._lstPlayers.Add(player.Id, player);
                }
            }
            finally
            {
                this._lckPlayers.ReleaseWriterLock();
            }
        }

        private void RemovePlayer(ClsPlayer player)
        {
            if (player == null) return;
            this._lckPlayers.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (this._lstPlayers.ContainsKey(player.Id))
                {
                    this._lstPlayers.Remove(player.Id);
                }
            }
            finally
            {
                this._lckPlayers.ReleaseWriterLock();
            }
        }

        void IEngineWrapper.ServerCommand(string command)
        {
            this.InterThreadCall<object, string>(this.ServerCommand_CALL, command);
        }

        private object ServerCommand_CALL(string command)
        {
            NativeMethods.Mono_ServerCommand(command);
            return null;
        }

        void IEngineWrapper.ClientMessage(ClsPlayer client, string message)
        {
            this.InterThreadCall<object, CTuple<int, string>>(this.ClientMessage_CALL, new CTuple<int, string>(client.Id, message));
        }

        private object ClientMessage_CALL(CTuple<int, string> data)
        {
            NativeMethods.Mono_ClientMessage(data.Item1, data.Item2);
            return null;
        }
    }
}
