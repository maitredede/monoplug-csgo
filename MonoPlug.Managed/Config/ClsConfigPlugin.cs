using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MonoPlug
{
    [XmlType("TPlugin")]
    public sealed class ClsConfigPlugin
    {
        private bool _load;
        private string _name;

        [XmlAttribute("loaded")]
        public bool Loaded { get { return this._load; } set { this._load = value; } }
        [XmlAttribute("name")]
        public string Name { get { return this._name; } set { this._name = value; } }
    }
}
