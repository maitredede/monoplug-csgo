using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [System.Diagnostics.DebuggerDisplay("#{Id} {Name}")]
    public sealed class PlayerData : IPlayer
    {
        public PlayerData() : base() { }

        internal PlayerData(NativePlayerData other)
            : this()
        {
            this.Id = other.id;
            this.Name = other.name;
            this.Team = other.team;
            this.Health = other.health;
            this.IpAddress = other.ip_address;
            this.SteamID = other.steam_id;

            this.IsBot = other.is_bot;
            this.IsDead = other.is_dead;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Team { get; set; }
        public int Health { get; set; }
        public bool IsBot { get; set; }
        public bool IsDead { get; set; }
        public string IpAddress { get; set; }
        public string SteamID { get; set; }
    }
}
