using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public static class LogExtensions
    {
        public static void ConfigureLog4Net(this PluginBase plugin)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(plugin.GetConfig().FilePath));
        }
    }
}
