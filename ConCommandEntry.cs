using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    internal struct ConCommandEntry
    {
        public ClsPluginBase Plugin;
        public string Name;
        public string Description;
        public ConCommandDelegate Code;
    }
}
