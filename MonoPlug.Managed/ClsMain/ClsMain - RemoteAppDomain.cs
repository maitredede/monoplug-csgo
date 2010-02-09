using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace MonoPlug
{
    partial class ClsMain
    {
        private AppDomain CreateAppDomain(string name, out ClsProxy proxy)
        {
            return this.CreateAppDomain(name, out proxy, false);
        }

        private AppDomain CreateAppDomain(string name, out ClsProxy proxy, bool shadow)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = this._assemblyPath;
            setup.ShadowCopyFiles = shadow.ToString();
            AppDomain dom = AppDomain.CreateDomain(name, null, setup);
            proxy = ClsProxy.CreateProxy(dom, this._msg);
            return dom;
        }
    }
}
