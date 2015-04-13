using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public static class CommandExtensions
    {
        public static Task<string> KickId(this IEngine engine, PlayerData player)
        {
            return engine.ExecuteCommand(string.Format("kickid {0}", player.Id));
        }
    }
}
