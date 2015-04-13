using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Player info Interface
    /// </summary>
    internal interface IPlayer
    {
        /// <summary>
        /// Gets the player Id.
        /// </summary>
        int Id { get; }
        string Name { get; }
        int Team { get; }
        int Health { get; }
        bool IsBot { get; }
        bool IsDead { get; }
        string IpAddress { get; }
        string SteamID { get; }
    }
}
