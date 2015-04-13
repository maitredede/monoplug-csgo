using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.LiveServer
{
    public interface IWebUIHubClient
    {
        Task RaiseEvent(string serverId, GameEventData e);
        Task SetPlayers(string serverId, PlayerData[] players);
    }
}
