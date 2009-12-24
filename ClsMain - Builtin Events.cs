
using System;

namespace MonoPlug
{
	partial class ClsMain
	{
		internal void EVT_LevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
		{
			Mono_Msg("M:EVT_LevelInit\n");
		}
		
		internal void EVT_LevelShutdown()
		{
			Mono_Msg("M:EVT_LevelShutdown\n");
		}
	}
}
