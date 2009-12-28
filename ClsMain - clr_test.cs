using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
#if DEBUG
        [ConCommand("clr_test", "Add the clr_untest command", FCVAR.FCVAR_GAMEDLL)]
        private void clr_test(string args)
        {
            //this.RegisterCommand(null, "clr_untest", "Remove the clr_test command", this.clr_untest, FCVAR.FCVAR_GAMEDLL);
            //Mono_Msg("clr_untest registered OK\n");
            Mono_Msg(string.Format("Current vartest value is {0}\n", this._clr_vartest.Value));
            this._clr_vartest.Value = DateTime.Now.ToLongTimeString();
        }

        //[ConCommand("clr_untest", "Remove the clr_test command", FCVAR.FCVAR_GAMEDLL)]
        //private void clr_untest(string args)
        //{
        //    this.UnregisterCommand(null, "clr_test");
        //    Mono_Msg("clr_test unregistred\n");
        //}

        private ClsConVarStrings _clr_vartest;
        private void _clr_vartest_changed(object sender, EventArgs e)
        {
            Mono_Msg(string.Format("M: VARTEST Value changed : {0}\n", this._clr_vartest.Value));
        }
#endif
    }
}
