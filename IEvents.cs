using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    public interface IEvents
    {
        void LevelShutdown_Add(EventHandler action);
        void LevelShutdown_Remove(EventHandler action);
    }
}
