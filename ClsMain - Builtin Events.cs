
using System;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void EVT_LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        {
            Mono_Msg("M:EVT_LevelInit\n");
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
        }
    }
}
