
using System;

namespace MonoPlug
{
    [Serializable]
    internal struct PluginDefinition
    {
        public string File { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("Plugin : Name='{0}' Type='{1}' File='{2}'\n   {3}", this.Name, this.Type, this.File, this.Description);
        }
    }
}
