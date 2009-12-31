using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
#if DEBUG
        private ClsConVarStrings _clr_vartest = null;

        //[ConCommand("clr_test", "Command for testing purposes, /!\\ unpredictable effects ahead /!\\", FCVAR.FCVAR_GAMEDLL)]
        private void clr_test(string args)
        {
            //this.RegisterCommand(null, "clr_untest", "Remove the clr_test command", this.clr_untest, FCVAR.FCVAR_GAMEDLL);
            //Mono_Msg("clr_untest registered OK\n");
            //Mono_Msg(string.Format("Current vartest value is {0}\n", this._clr_vartest.Value));
            //this._clr_vartest.Value = DateTime.Now.ToLongTimeString();
            if (this._clr_vartest == null)
            {
                Msg("M:Create var\n");
                this._clr_vartest = this.RegisterConVarString(null, "clr_var", "Test var", FCVAR.FCVAR_CHEAT, "je de lol");
                this._clr_vartest.ValueChanged += this._clr_vartest_changed;
            }
            else
            {
                Msg("M:Delete var\n");
                this.UnregisterConVarString(null, this._clr_vartest);
                this._clr_vartest = null;
            }
        }

        private void _clr_vartest_changed(object sender, EventArgs e)
        {
            Msg("M: VARTEST Value changed : {0}\n", this._clr_vartest.Value);
        }
#endif
    }
}
