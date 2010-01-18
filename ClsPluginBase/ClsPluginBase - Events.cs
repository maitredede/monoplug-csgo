using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MonoPlug
{
    partial class ClsPluginBase
    {
        private readonly object _lckEventCounters = new object();
        private readonly EventHandlerList _events = new EventHandlerList();
    }
}
