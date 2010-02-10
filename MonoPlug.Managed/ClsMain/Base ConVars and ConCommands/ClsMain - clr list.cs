using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void _clrList()
        {
            if (this._pluginCache == null)
                this._pluginCache = new PluginDefinition[] { };
            this._msg.Msg("Assembly path : {0}\n", this._assemblyPath);
            this._msg.Msg("Available     : {0} plugins\n", this._pluginCache.Length);
            foreach (PluginDefinition desc in this._pluginCache)
            {
                if (desc.DontList)
                    continue;
                this._msg.Msg("  {0}\n", desc);
            }

            this._lckPlugins.AcquireReaderLock(Timeout.Infinite);
            try
            {
                this._msg.Msg("Loaded : {0} plugins\n", this._plugins.Count);
                foreach (AppDomain dom in this._plugins.Keys)
                {
                    this._msg.Msg("  {0}\n", this._plugins[dom].Name);
                }
            }
            finally
            {
                this._lckPlugins.ReleaseReaderLock();
            }
        }
    }
}
