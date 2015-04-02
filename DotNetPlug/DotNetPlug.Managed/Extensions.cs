using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Append to game log
        /// </summary>
        /// <param name="engine">Game engine</param>
        /// <param name="format">String format</param>
        /// <param name="args">String formatting arguments</param>
        /// <returns></returns>
        public static Task Log(this IEngine engine, string format, params object[] args)
        {
            return engine.Log(string.Format(format, args));
        }
    }
}
