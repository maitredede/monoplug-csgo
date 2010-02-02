using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MonoPlug
{
    /// <summary>
    /// Config file : plugin entry
    /// </summary>
    [XmlType("TPlugin")]
    [XmlRoot("plugin")]
    [Obsolete("Remove", true)]
    public sealed class ClsConfigPlugin
    {
        private bool _load;
        private string _name;

        /// <summary>
        /// Plugin name
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get { return this._name; } set { this._name = value; } }
        /// <summary>
        /// Loaded
        /// </summary>
        [XmlAttribute("loaded")]
        public bool Loaded { get { return this._load; } set { this._load = value; } }
    }
}
