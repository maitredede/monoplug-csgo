﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    /// <summary>
    /// Interface for ThreadPool
    /// </summary>
    public interface IThreadPool
    {
        /// <summary>
        /// Queue an action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <param name="item"></param>
        void QueueUserWorkItem<T>(ThreadAction<T> work, T item);
        /// <summary>
        /// Queue an action
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="work"></param>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        void QueueUserWorkItem<T1, T2>(ThreadAction<T1, T2> work, T1 item1, T2 item2);
        /// <summary>
        /// Queue an action
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        void QueueUserWorkItem(WaitCallback callback, object state);
    }
}