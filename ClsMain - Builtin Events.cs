
using System;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void EVT_LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        {
            Mono_Msg("M:EVT_LevelInit\n");

			DumpThreadId();
			
            this.RegisterCommand(null, "clr_plugin_list", "List available plugins", this.clr_plugin_list, FCVAR.FCVAR_GAMEDLL);
            this.RegisterCommand(null, "clr_plugin_refresh", "Refresh plugin list", this.clr_plugin_refresh, FCVAR.FCVAR_GAMEDLL);
            this._clr_mono_version = this.RegisterConVarString(null, "clr_mono_version", "Get current Mono runtime version", FCVAR.FCVAR_GAMEDLL | FCVAR.FCVAR_SPONLY | FCVAR.FCVAR_CHEAT, ClsMain.MonoVersion);

#if DEBUG
            this.RegisterCommand(null, "clr_test", "Test clr", this.clr_test, FCVAR.FCVAR_GAMEDLL);
            this._clr_vartest = this.RegisterConVarString(null, "clr_vartest", "Test var for callbacks", FCVAR.FCVAR_GAMEDLL, string.Empty);
            this._clr_vartest.ValueChanged += this._clr_vartest_changed;
#endif

            lock (this._plugins)
            {
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    ClsPluginBase plugin = this._plugins[dom];
                    plugin.EVT_LevelInit(mapName, mapEntities, oldLevel, landmarkName, loadGame, background);
                }
            }
        }

        internal void EVT_LevelShutdown()
        {
            Mono_Msg("M:EVT_LevelShutdown\n");
            lock (this._plugins)
            {
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    ClsPluginBase plugin = this._plugins[dom];
                    plugin.EVT_LevelShutdown();
                }
            }

#if DEBUG
            this.UnregisterCommand(null, "clr_test");
            this.UnregisterConVarString(null, this._clr_vartest);
#endif
            this.UnregisterCommand(null, "clr_plugin_list");
            this.UnregisterCommand(null, "clr_plugin_refresh");

            this.UnregisterConVarString(null, this._clr_mono_version);
        }
    }
}
