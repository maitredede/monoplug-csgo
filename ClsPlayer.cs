using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MonoPlug
{
    [System.Diagnostics.DebuggerDisplay("#{Id} {Name}")]
    public sealed class ClsPlayer : MarshalByRefObject
    {
        private int _armor = 0;
        private int _death = 0;
        private int _frag = 0;
        private int _health = 0;
        private int _maxhealth = 0;
        private string _name = string.Empty;
        private string _steamid = string.Empty;
        private int _id = 0;
        private string _language = string.Empty;

        internal ClsPlayer()
        {
        }

        public int Armor { get { return this._armor; } }
        public int Death { get { return this._death; } }
        public int Frag { get { return this._frag; } }
        public int Health { get { return this._health; } }
        public int MaxHealth { get { return this._maxhealth; } }
        public string Name { get { return this._name; } }
        public string SteamID { get { return this._steamid; } }
        public int Id { get { return this._id; } }
        public string Language { get { return this._language; } }

        public override string ToString()
        {
            return string.Format("#{0} {1}", this._id, this._name);
        }
    }
}
