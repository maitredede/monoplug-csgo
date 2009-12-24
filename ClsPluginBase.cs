
using System;

namespace MonoPlug
{
	public abstract class ClsPluginBase:MarshalByRefObject
	{
		private ClsMain _main;
		
		internal void Init(ClsMain main)
		{
			this._main=main;
			this.Load();
		}
		
		internal void UnInit()
		{
			this.Unload();
		}
		
		protected abstract void Load();
		protected abstract void Unload();
		protected virtual void Pause(){}
		protected virtual void Unpause(){}
		
		public abstract string Name{get;}
		
		public ClsPluginBase()
		{
		}
		
		protected void Msg(string message)
		{
			this._main.Msg(message);
		}
		
		internal virtual void LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
		{
		}
	}
}
