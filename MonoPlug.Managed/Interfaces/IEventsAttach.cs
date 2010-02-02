using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal interface IEventsAttach
    {
        void LevelShutdown_Attach(ClsPluginBase plugin);
        void LevelShutdown_Detach(ClsPluginBase plugin);

        void ConMessage_Add(ClsPluginBase plugin);
        void ConMessage_Remove(ClsPluginBase plugin);

        void ClientPutInServer_Add(ClsPluginBase plugin);
        void ClientPutInServer_Remove(ClsPluginBase plugin);
    }
}
