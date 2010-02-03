using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    public interface IEvents
    {
        event EventHandler LevelShutdown;
        event EventHandler<ConMessageEventArgs> ConsoleMessage;
        event EventHandler<ClientEventArgs> ClientPutInServer;
        event EventHandler<ClientEventArgs> ClientDisconnect;
        event EventHandler<ServerActivateEventArgs> ServerActivate;
    }
}
