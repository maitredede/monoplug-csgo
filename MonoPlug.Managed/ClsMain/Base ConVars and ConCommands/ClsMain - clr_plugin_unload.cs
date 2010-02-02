using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void UnloadPlugin(AppDomain dom, ClsPluginBase plugin)
        {
            if (plugin == null)
            {
                return;
            }
            //Deinit plugin
            try
            {
                plugin.Uninit();
            }
            catch (Exception ex)
            {
                this._msg.Warning("Plugin UnInit() error\n");
                this._msg.Warning(ex);
            }

            //Clean convars and concommands
            this._lckConCommandBase.AcquireWriterLock(Timeout.Infinite);
            try
            {
                List<InternalConbase> lstRemove = new List<InternalConbase>();
                foreach (UInt64 nativeId in this._conCommandBase.Keys)
                {
                    InternalConbase cBase = this._conCommandBase[nativeId];
                    if (cBase.Plugin == plugin)
                    {
                        lstRemove.Add(cBase);
                    }
                }
                foreach (InternalConbase cBase in lstRemove)
                {
                    if (cBase is InternalConCommand)
                    {
                        this.InterThreadCall<object, UInt64>(this.UnregisterConCommand_Call, cBase.NativeID);
                    }
                    if (cBase is InternalConvar)
                    {
                        this.InterThreadCall<object, UInt64>(this.UnregisterConvar_Call, cBase.NativeID);
                    }
                    this._conCommandBase.Remove(cBase.NativeID);
                }
            }
            finally
            {
                this._lckConCommandBase.ReleaseWriterLock();
            }
        }

        private void clr_plugin_unload(string line, string[] arguments)
        {
            try
            {
                //Find if plugin is loaded
                bool found = false;
                this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        ClsPluginBase plugin = this._plugins[dom];
                        if (plugin.Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;

                            this.UnloadPlugin(dom, plugin);

                            //Clean plugin
                            string plugname = plugin.Name;
                            this._plugins.Remove(dom);
                            this._plugins.Remove(dom);
                            AppDomain.Unload(dom);

                            this._msg.Msg("Plugin '{0}' unloaded\n", plugname);
                            break;
                        }
                    }
                }
                finally
                {
                    this._lckPlugins.ReleaseWriterLock();
                }
                if (!found)
                {
                    this._msg.Msg("Plugin '{0}' not found or loaded\n", line);
                }
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
            }
        }
    }
}
