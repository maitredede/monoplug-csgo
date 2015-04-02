using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class LevelInitEventArgs : EventArgs
    {
        internal LevelInitEventArgs()
        {
        }
        
        public bool Background { get; internal set; }
        public string MapName { get; internal set; }
        public string MapEntities { get; internal set; }
        public string OldLevel { get; internal set; }
        public string LandmarkName { get; internal set; }
        public bool LoadGame { get; internal set; }
    }
}
