using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Interface of accessible methods for native lib
    /// </summary>
    internal interface IPluginManager
    {
        void Load();
        void Tick();
        void Unload();

        void RaiseCommand(int id, int argc, string[] argv);

        void RaiseLevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background);
        void RaiseServerActivate(int clientMax);

    }
}
