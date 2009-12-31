using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    [Obsolete("Use register function", true)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ConCommandAttribute : Attribute
    {
        private string _name;
        private string _description;
        private FCVAR _flags;

        public ConCommandAttribute(string name, string description, FCVAR flags)
        {
            this._name = name;
            this._description = description;
            this._flags = flags;
        }

        public string Name { get { return this._name; } }
        public string Description { get { return this._description; } }
        public FCVAR Flags { get { return this._flags; } }
    }
}
