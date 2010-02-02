using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Simple delegate for actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public delegate void ThreadAction<T>(T obj);
    /// <summary>
    /// Simple delegate for actions
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="item1"></param>
    /// <param name="item2"></param>
    public delegate void ThreadAction<T1, T2>(T1 item1, T2 item2);
}
