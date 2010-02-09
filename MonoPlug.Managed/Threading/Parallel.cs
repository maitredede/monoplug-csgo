using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;

namespace MonoPlug
{
    public static class Parallel
    {
        /// <summary>
        /// Exectute action on each item of collection
        /// </summary>
        /// <typeparam name="T">Type of collection item</typeparam>
        /// <param name="collection">Collection of items</param>
        /// <param name="action">Action to execute</param>
        /// <param name="threadsPerProc">Number of total threads</param>
        public static void ForEach<T>(ICollection<T> collection, Action<T> action) where T : class
        {
            Check.NonNull("collection", collection);
            Check.NonNull("action", action);

            ParallelForEach<T> task = new ParallelForEach<T>(collection, action, Environment.ProcessorCount);

            Dictionary<T, Exception> lstEx = task.Execute();

            if (lstEx.Count > 0)
            {
                throw new ParallelException<T>(lstEx);
            }
        }
    }
}
