using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsMain
    {
#if DEBUG
        private InternalConCommand _clr_test;

        private void clr_test(string line, string[] arguments)
        {
            //IList<ClsPlayer> players = this.GetPlayers();
            //this.Msg("Players count : {0}\n", players.Count);

            //for (int i = 0; i < players.Count; i++)
            //{
            //    this.Msg("#{0}\t{1}\t{2}\n", players[i].Id, players[i].Name, string.IsNullOrEmpty(players[i].IP) ? "<BOT>" : players[i].IP);
            //}
            //this.AllPluginsLoadedExecute();
        }
#endif
    }
}
