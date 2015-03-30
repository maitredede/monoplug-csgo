using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public static class Extensions
    {
        public static Task Log(this IEngine engine, string format, params object[] args)
        {
            return engine.Log(string.Format(format, args));
        }
    }
}
