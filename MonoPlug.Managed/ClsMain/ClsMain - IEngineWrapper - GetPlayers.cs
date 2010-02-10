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

        private void AddPlayer(ClsPlayer player)
        {
            if (player == null) return;
            this._lckPlayers.AcquireWriterLock(Timeout.Infinite);
            try
            {
                if (this._lstPlayers.ContainsKey(player.UserId))
                {
                    ClsPlayer current = this._lstPlayers[player.UserId];
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
                    this._lstPlayers.Add(player.UserId, player);
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
                if (this._lstPlayers.ContainsKey(player.UserId))
                {
                    this._lstPlayers.Remove(player.UserId);
                }
            }
            finally
            {
                this._lckPlayers.ReleaseWriterLock();
            }
        }

        private void ClearPlayer()
        {
            this._lckPlayers.AcquireWriterLock(Timeout.Infinite);
            try
            {
                this._lstPlayers.Clear();
            }
            finally
            {
                this._lckPlayers.ReleaseWriterLock();
            }
        }

        ClsPlayer[] IEngineWrapper.GetPlayers()
        {
            List<ClsPlayer> lst = new List<ClsPlayer>();
            this._lckPlayers.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (int id in this._lstPlayers.Keys)
                {
                    ClsPlayer player = this._lstPlayers[id];
                    if (player.IsConnecting || player.IsConnected)
                        lst.Add(player);
                }
                return lst.ToArray();
            }
            finally
            {
                this._lckPlayers.ReleaseReaderLock();
            }
        }
    }
}
