using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        //private readonly ReaderWriterLock _lckDicPlayers = new ReaderWriterLock();
        //private readonly Dictionary<int, ClsPlayer> _dicPlayers = new Dictionary<int, ClsPlayer>();

        //internal IList<ClsPlayer> GetPlayers()
        //{
        //    this._lckDicPlayers.AcquireReaderLock(Timeout.Infinite);
        //    try
        //    {
        //        List<ClsPlayer> lst = new List<ClsPlayer>(this._dicPlayers.Values);
        //        return lst.AsReadOnly();
        //    }
        //    finally
        //    {
        //        this._lckDicPlayers.ReleaseReaderLock();
        //    }
        //}

        //internal void AddPlayer(ClsPlayer player)
        //{
        //    this._lckDicPlayers.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        if (this._dicPlayers.ContainsKey(player.Id))
        //        {
        //            this._dicPlayers.Remove(player.Id);
        //        }
        //        this._dicPlayers.Add(player.Id, player);
        //    }
        //    finally
        //    {
        //        this._lckDicPlayers.ReleaseWriterLock();
        //    }
        //}

        //internal ClsPlayer RemovePlayer(int playerId)
        //{
        //    this._lckDicPlayers.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        ClsPlayer player = null;
        //        if (this._dicPlayers.ContainsKey(playerId))
        //        {
        //            player = this._dicPlayers[playerId];
        //            this._dicPlayers.Remove(playerId);
        //        }
        //        return player;
        //    }
        //    finally
        //    {
        //        this._lckDicPlayers.ReleaseWriterLock();
        //    }
        //}
    }
}
