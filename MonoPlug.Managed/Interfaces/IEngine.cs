using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    [Obsolete("Replaced by IEventsAttach", true)]
    internal interface IEngine
    {
        void LevelShutdown_Add(ClsPluginBase plugin);
        void LevelShutdown_Remove(ClsPluginBase plugin);

        void ClientPutInServer_Add(ClsPluginBase plugin);
        void ClientPutInServer_Remove(ClsPluginBase plugin);

        void ConMessage_Add(ClsPluginBase plugin);
        void ConMessage_Remove(ClsPluginBase plugin);
    }
}
