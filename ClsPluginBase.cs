
using System;

namespace MonoPlug
{
    public abstract class ClsPluginBase : MarshalByRefObject
    {
        private ClsMain _main;

        internal void Init(ClsMain main)
        {
            this._main = main;
            this.Load();
        }

        internal void UnInit()
        {
            this.Unload();
        }

        protected abstract void Load();
        protected abstract void Unload();
        protected virtual void Pause() { }
        protected virtual void Unpause() { }

        public abstract string Name { get; }

        public ClsPluginBase()
        {
        }

        protected void Msg(string message)
        {
            this._main.Msg(message);
        }

        internal void EVT_LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        {
            this.LevelInit(mapName, mapEntities, oldLevel, landmarkName, loadGame, background);
        }

        protected virtual void LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        {
        }

        internal void EVT_LevelShutdown()
        {
            this.LevelShutdown();
        }

        protected virtual void LevelShutdown()
        {
        }
    }
}
