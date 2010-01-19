using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsMain
    {
#if DEBUG
        private ClsConCommand _clr_test;

        private void clr_test(string line, string[] arguments)
        {
            IList<ClsPlayer> players = this.GetPlayers();
            this.Msg("Players count : {0}\n", players.Count);

            for (int i = 0; i < players.Count; i++)
            {
                this.Msg("#{0}\t{1}\t{2}\n", players[i].Id, players[i].Name, string.IsNullOrEmpty(players[i].IP) ? "<BOT>" : players[i].IP);
            }
        }
        //private ClsConvar _clr_vartest = null;

        //private void clr_test(string args)
        //{
        //    //this.RegisterCommand(null, "clr_untest", "Remove the clr_test command", this.clr_untest, FCVAR.FCVAR_GAMEDLL);
        //    //Mono_Msg("clr_untest registered OK\n");
        //    //Mono_Msg(string.Format("Current vartest value is {0}\n", this._clr_vartest.Value));
        //    //this._clr_vartest.Value = DateTime.Now.ToLongTimeString();
        //    if (this._clr_vartest == null)
        //    {
        //        Msg("M:Create var\n");
        //        this._clr_vartest = this.RegisterConvar(null, "clr_var", "Test var", FCVAR.FCVAR_CHEAT, "je de lol");
        //        this._clr_vartest.ValueChanged += this._clr_vartest_changed;
        //    }
        //    else
        //    {
        //        Msg("M:Delete var\n");
        //        this.UnregisterConvar(null, this._clr_vartest);
        //        this._clr_vartest = null;
        //    }
        //}

        //private void _clr_vartest_changed(object sender, EventArgs e)
        //{
        //    Msg("M: VARTEST Value changed : {0}\n", this._clr_vartest.GetString());
        //}
#endif
    }
}
