using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Exposed events that are handled
    /// </summary>
    public interface IEvents
    {
        /// <summary>
        /// Level Shutdown
        /// </summary>
        event EventHandler LevelShutdown;
        /// <summary>
        /// Console message
        /// </summary>
        event EventHandler<ConMessageEventArgs> ConsoleMessage;
        /// <summary>
        /// ClientPutInServer
        /// </summary>
        event EventHandler<ClientEventArgs> ClientPutInServer;
        /// <summary>
        /// ClientDisconnect
        /// </summary>
        event EventHandler<ClientEventArgs> ClientDisconnect;
        /// <summary>
        /// ServerActivate
        /// </summary>
        event EventHandler<ServerActivateEventArgs> ServerActivate;
        /// <summary>
        /// ServerSpawn
        /// </summary>
        event EventHandler<ServerSpawnEventArgs> ServerSpawn;
        /// <summary>
        /// ServerShutdown
        /// </summary>
        event EventHandler<ReasonEventArgs> ServerShutdown;
        /// <summary>
        /// PlayerConnect
        /// </summary>
        event EventHandler<ClientEventArgs> PlayerConnect;
        /// <summary>
        /// PlayerDisconnect
        /// </summary>
        event EventHandler<PlayerDisconnectEventArgs> PlayerDisconnect;
        /// <summary>
        /// ClientConnect
        /// </summary>
        event EventHandler<ClientEventArgs> ClientConnect;

    }
}
