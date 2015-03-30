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
        void Tick();
        void AllPluginsLoaded();
        void Unload();

        void LoadAssembly(string[] param);
    }
}
