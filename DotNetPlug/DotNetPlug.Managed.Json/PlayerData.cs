using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class PlayerData
    {
        public PlayerData() : base() { }

        internal PlayerData(IPlayer other)
            : this()
        {
            this.Id = other.Id;
            this.Name = other.Name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
