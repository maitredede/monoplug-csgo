using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace MonoPlug
{
    partial class ClsMain
    {
        private string GetAssemblyDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private AppDomain CreateAppDomain(string name, out ClsRemote proxy)
        {
            return this.CreateAppDomain(name, out proxy, false);
        }

        private AppDomain CreateAppDomain(string name, out ClsRemote proxy, bool shadow)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = this.GetAssemblyDirectory();
            setup.ShadowCopyFiles = shadow.ToString();
            AppDomain dom = AppDomain.CreateDomain(name, null, setup);
            proxy = ClsRemote.CreateInDomain<ClsRemote>(dom, this._msg);
            return dom;
        }
    }
}
