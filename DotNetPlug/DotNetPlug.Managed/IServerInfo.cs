using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Server info
    /// </summary>
    public interface IServerInfo
    {
        int AppId { get; }
        string GameDir { get; }
        string[] LaunchOptions { get; }
        int Version { get; }
        float TimeScale { get; }
        bool IsDedicatedServer { get; }
        string GameDescription { get; }
        int TickInterval { get; }

        int PlayerMin { get; }
        int PlayerMax { get; }
    }
}
