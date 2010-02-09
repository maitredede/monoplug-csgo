using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal interface IEventsAnchor
    {
        void LevelShutdown_Attach(ClsPluginBase plugin);
        void LevelShutdown_Detach(ClsPluginBase plugin);

        void ConsoleMessage_Attach(ClsPluginBase plugin);
        void ConsoleMessage_Detach(ClsPluginBase plugin);

        void ClientConnect_Attach(ClsPluginBase plugin);
        void ClientConnect_Detach(ClsPluginBase plugin);

        void ClientPutInServer_Attach(ClsPluginBase plugin);
        void ClientPutInServer_Detach(ClsPluginBase plugin);

        void ClientDisconnect_Attach(ClsPluginBase plugin);
        void ClientDisconnect_Detach(ClsPluginBase plugin);

        void ServerActivate_Attach(ClsPluginBase plugin);
        void ServerActivate_Detach(ClsPluginBase plugin);

        void ServerSpawn_Attach(ClsPluginBase plugin);
        void ServerSpawn_Detach(ClsPluginBase plugin);

        void ServerShutdown_Attach(ClsPluginBase plugin);
        void ServerShutdown_Detach(ClsPluginBase plugin);

        void PlayerConnect_Attach(ClsPluginBase plugin);
        void PlayerConnect_Detach(ClsPluginBase plugin);

        void PlayerDisconnect_Attach(ClsPluginBase plugin);
        void PlayerDisconnect_Detach(ClsPluginBase plugin);
    }
}
