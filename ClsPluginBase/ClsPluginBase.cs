
using System;

namespace MonoPlug
{
    public abstract partial class ClsPluginBase : MarshalByRefObject
    {
        private ClsMain _main;

        internal void Init(ClsMain main)
        {
#if DEBUG
            main.Msg("ClsPluginBase:: Init in AppDomain '{0}'\n", AppDomain.CurrentDomain.FriendlyName);
#endif

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
        public abstract string Description { get; }

        public ClsPluginBase()
        {
        }

        protected void Msg(string format, params object[] args)
        {
            this._main.Msg(format, args);
        }

        //[Obsolete("Replace with hooked events")]
        //internal void EVT_LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        //{
        //    this.LevelInit(mapName, mapEntities, oldLevel, landmarkName, loadGame, background);
        //}

        //[Obsolete("Replace with hooked events")]
        //protected virtual void LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        //{
        //}

        //protected ClsConVarStrings RegisterConVarString(string name, string description, FCVAR flags, string defaultValue)
        //{
        //    return this._main.RegisterConVarString(this, name, description, flags, defaultValue);
        //}

        //protected void UnregisterConVarString(ClsConVarStrings convar)
        //{
        //    this._main.UnregisterConVarString(this, convar);
        //}

        protected ClsConvar RegisterConvar(string name, string help, FCVAR flags, string defaultValue)
        {
            return this._main.RegisterConvar(this, name, help, flags, defaultValue);
        }

        protected void UnregisterConvar(ClsConvar var)
        {
            this._main.UnregisterConvar(this, var);
        }

        protected ClsPlayer[] GetPlayers()
        {
            return this._main.GetPlayers();
        }
    }
}
