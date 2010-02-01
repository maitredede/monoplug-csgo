using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    public delegate void ThreadAction<T>(T obj);
    public delegate void ThreadAction<T1, T2>(T1 item1, T2 item2);
}
