using System;

namespace MonoPlug
{
    partial class ClsMain
    {
        /// <summary>
        /// Init function for main instance 
        /// </summary>
        internal bool Init()
        {
            //get current thread Id to check for interthread calls
            this._mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            this.InitEvents();

            //Register base commands and vars
            this._clr = ((IEngineWrapper)this).RegisterConCommand("clr", "MonoPlugin control options", FCVAR.FCVAR_NONE, this.clr, null, false);

            this._clr_plugin_directory = ((IEngineWrapper)this).RegisterConvar("clr_plugin_directory", "Assembly plugin search path", FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT | FCVAR.FCVAR_PRINTABLEONLY, this._assemblyPath);
#if DEBUG
            this._clr_test = ((IEngineWrapper)this).RegisterConCommand("clr_test", "for developpement purposes only", FCVAR.FCVAR_NONE, this.clr_test, null, false);
#endif
            return true;
        }
    }
}
