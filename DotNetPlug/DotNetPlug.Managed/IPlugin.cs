﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public interface IPlugin
    {
        void Init(IEngine engine);

        Task Load();
        Task Unload();
    }
}
