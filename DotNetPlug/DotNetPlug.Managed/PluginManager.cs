using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private IEngine m_engine;
        private readonly List<IPlugin> m_plugins = new List<IPlugin>();
        private readonly TaskScheduler m_taskScheduler;
        private readonly Assembly m_thisAssembly;

        internal SynchronizationContext SynchronizationContext { get { return this.m_syncCtx; } }
        internal TaskScheduler TaskScheduler { get { return this.m_taskScheduler; } }

        private PluginManager()
        {
            this.m_syncCtx = new EngineSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(this.m_syncCtx);
            this.m_taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomain_AssemblyResolve;
            this.m_thisAssembly = this.GetType().Assembly;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (this.m_engine != null)
            {
                AssemblyName name = new AssemblyName(args.Name);
                if (name.Name == this.m_thisAssembly.GetName().Name)
                    return this.m_thisAssembly;
                this.m_engine.Log("AppDomain.AssemblyResolve : ", args.Name);
            }
            return null;
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
            this.m_engine.Log("Hello from DotNetPlug PluginManager");

            //Type[] plugTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IPlugin).IsAssignableFrom(t))).ToArray();
            //foreach (Type tPlugin in plugTypes)
            //{
            //    if (tPlugin.GetConstructor(Type.EmptyTypes) != null)
            //    {
            //        IPlugin plugin = (IPlugin)Activator.CreateInstance(tPlugin);
            //        plugin.Init(this.m_engine);
            //        this.m_plugins.Add(plugin);
            //    }
            //}

            //foreach (IPlugin plugin in this.m_plugins)
            //{
            //    this.m_engine.m_cb_Log(string.Format("PluginManager : Loading plugin {0}", plugin.GetType().FullName));

            //    plugin.Load();
            //}
        }

        void IPluginManager.Unload()
        {
            foreach (IPlugin plugin in this.m_plugins)
            {
                plugin.Unload();
            }
        }


        internal void InitWin32Engine(Int64 cbLog, Int64 cbExecuteCommand, Int64 cbRegisterCommand)
        {
            Engine_Win32 eng = new Engine_Win32(this);
            eng.m_cb_Log = (LogDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(cbLog), typeof(LogDelegate));
            eng.m_cb_ExecuteCommand = (ExecuteCommandDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(cbExecuteCommand), typeof(ExecuteCommandDelegate));
            eng.m_cb_RegisterCommand = (RegisterCommandDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(cbRegisterCommand), typeof(RegisterCommandDelegate));
            this.m_engine = eng;
        }

        internal void InitMonoEngine()
        {
            Engine_Mono eng = new Engine_Mono(this);
            this.m_engine = eng;
        }


        async void IPluginManager.LoadAssembly(string[] param)
        {
            //invalid call : no args
            if (param == null)
                return;
            //invalid call : first arg is command
            if (param.Length == 0)
                return;

            if (param.Length != 2)
            {
                await this.m_engine.Log("Syntax : {0} <assemblyfile>", param[0]);
                return;
            }
            string assemblyFile = param[1];
            string assemblyPath = Path.GetFullPath(assemblyFile);
            await this.m_engine.Log("Trying to load assembly : {0}", assemblyPath);
            FileInfo fi = new FileInfo(assemblyPath);
            if (!fi.Exists)
                await this.m_engine.Log("Assembly file not found : {0}", assemblyPath);
            Assembly asm;
            try
            {
                asm = Assembly.LoadFile(fi.FullName);
            }
            catch (Exception ex)
            {
                asm = null;
                this.m_engine.Log("Assembly file loading error : {0}", ex.Message).Wait();
            }
            if (asm != null)
                await this.LoadPluginsFromAssembly(asm);
            await this.m_engine.Log("load_assembly end");
        }

        private async Task LoadPluginsFromAssembly(Assembly asm)
        {
            try
            {
                Type[] plugTypes = asm.GetTypes().Where(t => t.IsClass && !t.IsAbstract && typeof(IPlugin).IsAssignableFrom(t)).ToArray();
                foreach (Type tPlugin in plugTypes)
                {
                    if (tPlugin.GetConstructor(Type.EmptyTypes) != null)
                    {
                        await this.m_engine.Log("Trying to create plugin {0}", tPlugin.FullName);
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(tPlugin);
                        plugin.Init(this.m_engine);
                        this.m_plugins.Add(plugin);
                    }
                }

                foreach (IPlugin plugin in this.m_plugins)
                {
                    await this.m_engine.Log("PluginManager : Loading plugin {0}", plugin.GetType().FullName);

                    await plugin.Load();
                }
            }
            catch (Exception ex)
            {
                this.m_engine.Log("PluginManager : LoadPluginsFromAssembly error : {0}", ex.Message).Wait();
            }
        }

        void IPluginManager.RaiseCommand(int id, int argc, string[] argv)
        {
            this.m_engine.RaiseCommand(id, argc, argv);
        }
    }
}
