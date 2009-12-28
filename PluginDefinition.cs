
using System;

namespace MonoPlug
{
    internal sealed class PluginDefinition
    {
        public string File;
        public string Type;
        public string Name;

        public PluginDefinition Clone()
        {
            return (PluginDefinition)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return string.Format("Plugin : Name={0} Type={1} File={2}", this.Name, this.Type, this.File);
        }
    }
}
