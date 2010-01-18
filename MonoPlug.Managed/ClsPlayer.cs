using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace MonoPlug
{
    /// <summary>
    /// Player data
    /// </summary>
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
        private string _ip = string.Empty;
        private float _avgLatency = 0;
        private float _timeConnected = 0;

        internal ClsPlayer()
        {
        }

        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get { return this._id; } }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return this._name; } }
        /// <summary>
        /// Frags
        /// </summary>
        public int Frag { get { return this._frag; } }
        /// <summary>
        /// Death
        /// </summary>
        public int Death { get { return this._death; } }
        /// <summary>
        /// Armor
        /// </summary>
        public int Armor { get { return this._armor; } }
        /// <summary>
        /// Health
        /// </summary>
        public int Health { get { return this._health; } }
        /// <summary>
        /// Maximum health
        /// </summary>
        public int MaxHealth { get { return this._maxhealth; } }
        /// <summary>
        /// SteamID
        /// </summary>
        public string SteamID { get { return this._steamid; } }
        /// <summary>
        /// Client language
        /// </summary>
        public string Language { get { return this._language; } }
        /// <summary>
        /// Client IP
        /// </summary>
        public string IP { get { return this._ip; } }
        /// <summary>
        /// Average player latency
        /// </summary>
        public float AverageLatency { get { return this._avgLatency; } }
        /// <summary>
        /// Time player is connected
        /// </summary>
        public float TimeConnected { get { return this._timeConnected; } }

        /// <summary>
        /// Get a string summary of player
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("#{0} {1}", this._id, this._name);
        }
    }
}
