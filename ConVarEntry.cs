using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    internal struct ConVarEntry
    {
        public ClsPluginBase Plugin;
        public string Name;
        public string Description;
        public ConVarStringGetDelegate Get;
        public ConVarStringSetDelegate Set;
    }
}
