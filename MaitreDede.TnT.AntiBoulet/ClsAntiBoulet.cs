using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoPlug;
using System.Threading;

namespace MaitreDede.TnT.AntiBoulet
{
    public sealed class ClsAntiBoulet : ClsPluginBase
    {
        //private Timer _timer;

        public override string Name
        {
            get { return "TnT-AntiBoulet"; }
        }

        public override string Description
        {
            get { return "TnT: Dump dans la console des admins les infos des joueurs, pour gérer les boulets"; }
        }

        protected override void Load()
        {
            //this._timer = new Timer(this.Tick, null, 1, 30000);
            this.Events.ClientDisconnect += this.Events_ClientDisconnect;
            this.Events.PlayerDisconnect += this.Events_PlayerDisconnect;
        }

        protected override void Unload()
        {
            //this._timer.Dispose();
            this.Events.ClientDisconnect -= this.Events_ClientDisconnect;
            this.Events.PlayerDisconnect -= this.Events_PlayerDisconnect;
        }

        //private void Tick(object state)
        //{
        //    this.Message.Msg("TNTAB: Tick\n");
        //    try
        //    {
        //        ClsPlayer[] players = this.Engine.GetPlayers();
        //        ClsAdminEntry[] admins = this.Database.GetAdmins();

        //        this.Message.Msg("Admins count: {0} Players count: {1}\n", admins.Length, players.Length);

        //        foreach (ClsAdminEntry admin in admins)
        //        {
        //            foreach (ClsPlayer player in players)
        //            {
        //                if (player.IsAdmin(admin))
        //                {
        //                    this.Message.Msg("Player {0} is admin\n", player);

        //                    this.Engine.ClientPrint(player, "TNT-AB: *******************************\n");
        //                    foreach (ClsPlayer dumped in players)
        //                    {
        //                        this.Engine.ClientPrint(player, "TNT-AB: Name={0}\n", dumped.Name);
        //                        this.Engine.ClientPrint(player, "TNT-AB: Steam={0}\n", dumped.SteamID);
        //                        this.Engine.ClientPrint(player, "TNT-AB: IP={0}\n", dumped.IP);
        //                    }
        //                    this.Engine.ClientPrint(player, "TNT-AB: *******************************\n");
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Message.Warning(ex);
        //    }
        //}

        private void Events_ClientDisconnect(object sender, ClientEventArgs e)
        {
            this.Disconnect(e, null);
        }

        private void Events_PlayerDisconnect(object sender, PlayerDisconnectEventArgs e)
        {
            this.Disconnect(e, e.Reason);
        }

        private void Disconnect(ClientEventArgs e, string reason)
        {
            ClsAdminEntry[] admins = this.Database.GetAdmins();
            List<ClsPlayer> players = new List<ClsPlayer>(this.Engine.GetPlayers());

            this.Message.Msg("Admins count: {0} Players count: {1}\n", admins.Length, players.Count);

            foreach (ClsAdminEntry admin in admins)
            {
                foreach (ClsPlayer player in players)
                {
                    if (player.IsAdmin(admin))
                    {
                        this.Message.Msg("Player {0} is admin\n", player);
                        this.Engine.ClientPrint(player, "TNT-AB: ***** Client disconnected *****\n");
                        if (!string.IsNullOrEmpty(reason))
                            this.Engine.ClientPrint(player, "TNT-AB: Reason={0}\n", reason);
                        this.Engine.ClientPrint(player, "TNT-AB: Name={0}\n", e.Client.Name);
                        this.Engine.ClientPrint(player, "TNT-AB: Steam={0}\n", e.Client.SteamID);
                        this.Engine.ClientPrint(player, "TNT-AB: IP={0}\n", e.Client.IP);
                        this.Engine.ClientPrint(player, "TNT-AB: *******************************\n");
                        players.Remove(player);
                        break;
                    }
                }
            }
        }
    }
}
