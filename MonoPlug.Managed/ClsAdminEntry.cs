using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Database admin
    /// </summary>
    [Serializable]
    public sealed class ClsAdminEntry
    {
        internal ClsAdminEntry()
        {
        }

        private uint _id;
        private AuthType _authtype;
        private string _identity;
        private string _password;
        private string _flags;
        private string _name;
        private uint _immunity;

        /// <summary>
        /// Entry Id
        /// </summary>
        public uint Id { get { return this._id; } internal set { this._id = value; } }
        /// <summary>
        /// Authentication type
        /// </summary>
        public AuthType AuthType { get { return this._authtype; } internal set { this._authtype = value; } }
        /// <summary>
        /// Authentication data
        /// </summary>
        public string Identity { get { return this._identity; } internal set { this._identity = value; } }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get { return this._password; } internal set { this._password = value; } }
        /// <summary>
        /// Admin flags
        /// </summary>
        public string Flags { get { return this._flags; } internal set { this._flags = value; } }
        /// <summary>
        /// Admin name
        /// </summary>
        public string Name { get { return this._name; } internal set { this._name = value; } }
        /// <summary>
        /// Admin immunities
        /// </summary>
        public uint Immunity { get { return this._immunity; } internal set { this._immunity = value; } }

    }
}
