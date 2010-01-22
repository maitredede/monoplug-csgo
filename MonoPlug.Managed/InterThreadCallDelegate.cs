using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    internal delegate TRet InterThreadCallDelegate<TRet, TParam>(TParam param);
    internal delegate void Action<T>(T obj);
}
