using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    internal struct ConCommandEntry
    {
        public ClsPluginBase Plugin { get; set; }
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public ConCommandDelegate Code { get; set; }
        public ClsConCommand Command { get; set; }
    }
}
