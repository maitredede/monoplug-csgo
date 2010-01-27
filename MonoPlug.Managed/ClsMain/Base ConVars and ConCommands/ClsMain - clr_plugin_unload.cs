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
                plugin.UnInit();
            }
            catch (Exception ex)
            {
                this.Warning("Plugin UnInit() error\n");
                this.Warning(ex);
            }

            //Clean convars and concommands
            this._lckConCommandBase.AcquireWriterLock(Timeout.Infinite);
            try
            {
                List<ClsConCommandBase> lstRemove = new List<ClsConCommandBase>();
                foreach (UInt64 nativeId in this._conCommandBase.Keys)
                {
                    ClsConCommandBase cBase = this._conCommandBase[nativeId];
                    if (cBase.Plugin == plugin)
                    {
                        lstRemove.Add(cBase);
                    }
                }
                foreach (ClsConCommandBase cBase in lstRemove)
                {
                    if (cBase is ClsConCommand)
                    {
                        this.InterThreadCall<object, UInt64>(this.UnregisterConCommand_Call, cBase.NativeID);
                    }
                    if (cBase is ClsConVar)
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
            NativeMethods.Mono_DevMsg("M: clr_plugin_unload 0\n");
            try
            {
                this.DevMsg("M: clr_plugin_unload A\n");
                //Find if plugin is loaded
                bool found = false;
                this._lckPlugins.AcquireWriterLock(Timeout.Infinite);
                try
                {
                    foreach (AppDomain dom in this._plugins.Keys)
                    {
                        this.DevMsg("M: clr_plugin_unload B\n");
                        ClsPluginBase plugin = this._plugins[dom];
                        this.DevMsg("M: clr_plugin_unload C {0}\n", plugin.Name);
                        if (plugin.Name.Equals(line, StringComparison.InvariantCultureIgnoreCase))
                        {
                            found = true;
                            this.DevMsg("M: clr_plugin_unload D\n");

                            this.UnloadPlugin(dom, plugin);

                            this.DevMsg("M: clr_plugin_unload I\n");
                            //Clean plugin
                            string plugname = plugin.Name;
                            this._plugins.Remove(dom);
                            this.DevMsg("M: clr_plugin_unload J\n");
                            this._plugins.Remove(dom);
                            AppDomain.Unload(dom);

                            this.Msg("Plugin '{0}' unloaded\n", plugname);
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
                    this.Msg("Plugin '{0}' not found or loaded\n", line);
                }
            }
            catch (Exception ex)
            {
                this.Warning(ex);
            }
        }
    }
}
