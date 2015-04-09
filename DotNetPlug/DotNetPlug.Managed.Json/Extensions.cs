using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public static class Extensions
    {
        public static GameEventData ToData(this GameEventEventArgs e)
        {
            if (e == null)
                return null;
            GameEventData data = new GameEventData(e);
            return data;
        }
    }
}
