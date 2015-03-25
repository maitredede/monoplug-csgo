using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal interface IPlugin
    {
        void Init(IEngine engine);

        Task Load();
        Task Unload();
    }
}
