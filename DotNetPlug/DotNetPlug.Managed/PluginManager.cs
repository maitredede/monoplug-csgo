using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class PluginManager : IPluginManager, IDisposable
    {
        private static PluginManager s_instance;

        internal static PluginManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (typeof(PluginManager))
                    {
                        if (s_instance == null)
                        {
                            s_instance = new PluginManager();
                        }
                    }
                }
                return s_instance;
            }
        }

        private readonly EngineSynchronizationContext m_syncCtx;
        private readonly EngineWrapper m_engine;
        private readonly List<IPlugin> m_plugins = new List<IPlugin>();
        private readonly TaskScheduler m_taskScheduler;

        internal SynchronizationContext SynchronizationContext { get { return this.m_syncCtx; } }
        internal TaskScheduler TaskScheduler { get { return this.m_taskScheduler; } }

        private PluginManager()
        {
            this.m_syncCtx = new EngineSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(this.m_syncCtx);
            this.m_taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            this.m_engine = new EngineWrapper(this);
        }

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }

        internal void WorkError(Exception ex)
        {
            throw new NotImplementedException();
        }

        void IPluginManager.Tick()
        {
            this.m_syncCtx.Tick();
        }

        void IPluginManager.AllPluginsLoaded()
        {
            Type[] plugTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IPlugin).IsAssignableFrom(t))).ToArray();
            foreach (Type tPlugin in plugTypes)
            {
                if (tPlugin.GetConstructor(Type.EmptyTypes) != null)
                {
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(tPlugin);
                    plugin.Init(this.m_engine);
                    this.m_plugins.Add(plugin);
                }
            }

            foreach (IPlugin plugin in this.m_plugins)
            {
                plugin.Load();
            }
        }

        void IPluginManager.Unload()
        {
            foreach (IPlugin plugin in this.m_plugins)
            {
                plugin.Unload();
            }
        }

        void IPluginManager.SetCallback_Log(Int64 callbackLog)
        {
            LogDelegate cb = (LogDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(callbackLog), typeof(LogDelegate));
            this.m_engine.m_cb_Log = cb;
        }

        void IPluginManager.SetCallback_ExecuteCommand(Int64 callbackLog)
        {
            ExecuteCommandDelegate cb = (ExecuteCommandDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(callbackLog), typeof(ExecuteCommandDelegate));
            this.m_engine.m_cb_ExecuteCommand = cb;
        }

    }
}
