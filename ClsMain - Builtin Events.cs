
using System;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal void EVT_LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        {
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
