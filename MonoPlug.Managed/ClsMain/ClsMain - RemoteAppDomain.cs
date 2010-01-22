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
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = this.GetAssemblyDirectory();
            //setup.ShadowCopyFiles = true.ToString();
            AppDomain dom = AppDomain.CreateDomain(name, null, setup);
            //AppDomain dom = AppDomain.CreateDomain(name);
            //proxy = this.RemoteCreateClass<ClsRemote>(this, dom);
            proxy = ClsRemote.CreateInDomain<ClsRemote>(dom, this);
#if DEBUG
            this.DevMsg("DBG: Fully created domain and proxy for domain '{0}'\n", name);
#endif
            return dom;
        }
    }
}
