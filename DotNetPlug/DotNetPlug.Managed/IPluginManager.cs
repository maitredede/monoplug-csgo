using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        //void SetCallback_Log([MarshalAs(UnmanagedType.FunctionPtr)] Action<string> callback);
        void SetCallback_Log(Int64 callback);
        void SetCallback_ExecuteCommand(Int64 callback);

    }
}
