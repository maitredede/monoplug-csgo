using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Reflection;

namespace MonoPlug
{
    /// <summary>
    /// Player data
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("#{Id} {Name}")]
    public sealed class ClsPlayer : ObjectBase
    {
        private int _userId = 0;
        private bool _isBot = false;
        private bool _isConnecting = false;
        private bool _isConnected = false;
        private string _name = string.Empty;
        private string _ip = string.Empty;
        private string _steamId = string.Empty;
        private bool _isSteamValid = false;

        private int _armor = 0;
        private int _death = 0;
        private int _frag = 0;
        private int _health = 0;
        private int _maxhealth = 0;
        private string _language = string.Empty;
        private float _avgLatency = 0;
        private float _timeConnected = 0;

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get { return this._userId; } }
        public bool IsBot { get { return this._isBot; } }
        public bool IsConnecting { get { return this._isConnecting; } }
        public bool IsConnected { get { return this._isConnected; } }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get { return this._name; } }
        /// <summary>
        /// Client IP
        /// </summary>
        public string IP { get { return this._ip; } }
        /// <summary>
        /// SteamID
        /// </summary>
        public string SteamID { get { return this._steamId; } }
        public bool IsSteamValid { get { return this._isSteamValid; } }

        internal ClsPlayer()
        {
        }

        /// <summary>
        /// Frags
        /// </summary>
        public int Frag { get { return this._frag; } internal set { this._frag = value; } }
        /// <summary>
        /// Death
        /// </summary>
        public int Death { get { return this._death; } internal set { this._death = value; } }
        /// <summary>
        /// Armor
        /// </summary>
        public int Armor { get { return this._armor; } internal set { this._armor = value; } }
        /// <summary>
        /// Health
        /// </summary>
        public int Health { get { return this._health; } internal set { this._health = value; } }
        /// <summary>
        /// Maximum health
        /// </summary>
        public int MaxHealth { get { return this._maxhealth; } internal set { this._maxhealth = value; } }
        /// <summary>
        /// Client language
        /// </summary>
        public string Language { get { return this._language; } internal set { this._language = value; } }
        /// <summary>
        /// Average player latency
        /// </summary>
        public float AverageLatency { get { return this._avgLatency; } internal set { this._avgLatency = value; } }
        /// <summary>
        /// Time player is connected
        /// </summary>
        public float TimeConnected { get { return this._timeConnected; } internal set { this._timeConnected = value; } }

        /// <summary>
        /// Get a string summary of player
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("#{0} {1}", this._userId, this._name);
        }

        /// <summary>
        /// Test if player matches admin entry (currently only SteamID)
        /// </summary>
        /// <param name="admin">Admin entry</param>
        /// <returns>True if player match admin entry</returns>
        public bool IsAdmin(ClsAdminEntry admin)
        {
            //TODO : check more admin AuthType
            switch (admin.AuthType)
            {
                case AuthType.steam: return this.SteamID == admin.Identity;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Full string representation of player
        /// </summary>
        /// <returns></returns>
        public string Dump()
        {
            List<string> lst = new List<string>();
            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                if (prop.CanRead)
                {
                    lst.Add(string.Format("{0}={1}", prop.Name, prop.GetValue(this, null) ?? (object)"<null>"));
                }
            }
            return string.Join(", ", lst.ToArray());
        }
    }
}
