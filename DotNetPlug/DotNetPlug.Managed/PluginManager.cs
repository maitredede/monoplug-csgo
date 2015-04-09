using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private EngineWrapperBase m_engine;
        private readonly List<IPlugin> m_plugins = new List<IPlugin>();
        private readonly TaskScheduler m_taskScheduler;
        private readonly Assembly m_thisAssembly;
        private CorePlugin m_corePlugin;

        internal SynchronizationContext SynchronizationContext { get { return this.m_syncCtx; } }
        internal TaskScheduler TaskScheduler { get { return this.m_taskScheduler; } }
        internal EngineWrapperBase EngineWrapper { get { return this.m_engine; } }

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

                string callerPath = Path.GetDirectoryName(args.RequestingAssembly.Location);

                string searchedPath = Path.Combine(callerPath, name.Name + ".dll");
                if (File.Exists(searchedPath))
                {
                    return Assembly.LoadFile(searchedPath);
                }

                this.m_engine.Log("AppDomain.AssemblyResolve : {0}", args.Name);
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

        void IPluginManager.Load()
        {
            this.m_corePlugin = new CorePlugin(this);
            ((IPlugin)this.m_corePlugin).Init(this.m_engine);
            this.m_corePlugin.LoadSync();
            //this.m_engine.Log("Hello from DotNetPlug PluginManager");

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

        async void IPluginManager.Unload()
        {
            foreach (IPlugin plugin in this.m_plugins)
            {
                await plugin.Unload();
            }
            this.m_corePlugin.UnloadSync();
        }

        internal void InitWin32Engine(Int64 cbLog, Int64 cbExecuteCommand, Int64 cbRegisterCommand, Int64 cbUnregisterCommand)
        {
            Engine_Win32 eng = new Engine_Win32(this);
            eng.m_cb_Log = (LogDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(cbLog), typeof(LogDelegate));
            eng.m_cb_ExecuteCommand = (ExecuteCommandDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(cbExecuteCommand), typeof(ExecuteCommandDelegate));
            eng.m_cb_RegisterCommand = (RegisterCommandDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(cbRegisterCommand), typeof(RegisterCommandDelegate));
            eng.m_cb_UnregisterCommand = (UnregisterCommandDelegate)Marshal.GetDelegateForFunctionPointer(new IntPtr(cbUnregisterCommand), typeof(UnregisterCommandDelegate));
            this.m_engine = eng;
        }

        internal void InitMonoEngine()
        {
            Engine_Mono eng = new Engine_Mono(this);
            this.m_engine = eng;
        }

        internal async void LoadAssembly(string[] param)
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
                    await this.CreatePluginAndLoad(tPlugin);
                    //if (tPlugin.GetConstructor(Type.EmptyTypes) == null)
                    //    continue;

                    //await this.m_engine.Log("Trying to create plugin {0}", tPlugin.FullName);
                    //IPlugin plugin = (IPlugin)Activator.CreateInstance(tPlugin);
                    //plugin.Init(this.m_engine);
                    //this.m_plugins.Add(plugin);

                    //await this.m_engine.Log("PluginManager : Loading plugin {0}", plugin.GetType().FullName);

                    //await plugin.Load();
                }
            }
            catch (Exception ex)
            {
                this.m_engine.Log("PluginManager : LoadPluginsFromAssembly error : {0}", ex.Message).Wait();
            }
        }

        private async Task<IPlugin> CreatePluginAndLoad(Type tPlugin)
        {
            if (tPlugin.GetConstructor(Type.EmptyTypes) == null)
                return null;

            await this.m_engine.Log("Trying to create plugin {0}", tPlugin.FullName);
            IPlugin plugin = (IPlugin)Activator.CreateInstance(tPlugin);
            plugin.Init(this.m_engine);
            lock (this.m_plugins)
            {
                this.m_plugins.Add(plugin);
            }

            await this.m_engine.Log("PluginManager : Loading plugin {0}", plugin.GetType().FullName);

            await plugin.Load();

            return plugin;
        }

        internal async void LoadType(string[] args)
        {
            if (args == null || args.Length < 1)
                return;
            if (args.Length != 2)
            {
                await this.EngineWrapper.Log("Usage : {0} <type>", args[0]);
            }

            Type t;
            try
            {
                t = Type.GetType(args[1]);
            }
            catch (Exception ex)
            {
                this.m_engine.Log("Can't get type {0} : {1}:{2}", args[1], ex.GetType().Name, ex.Message).Wait();
                return;
            }

            try
            {
                IPlugin plugin = await this.CreatePluginAndLoad(t);
            }
            catch (Exception ex)
            {
                this.m_engine.Log("Can't create plugin {0} : {1}:{2}", args[1], ex.GetType().Name, ex.Message).Wait();
                return;
            }
        }


        void IPluginManager.RaiseCommand(int id, int argc, string[] argv)
        {
            this.m_engine.RaiseCommand(id, argc, argv);
        }

        private void Raise<T>(Action<T> engineRaise, T args) where T : EventArgs
        {
            ThreadPool.QueueUserWorkItem((s) => engineRaise((T)s), args);
        }

        void IPluginManager.RaiseLevelInit(string mapName, string mapEntities, string oldLevel, string landmarkName, bool loadGame, bool background)
        {
            LevelInitEventArgs e = new LevelInitEventArgs()
            {
                MapName = mapName,
                MapEntities = mapEntities,
                OldLevel = oldLevel,
                LandmarkName = landmarkName,
                LoadGame = loadGame,
                Background = background,
            };
            this.Raise(this.m_engine.RaiseLevelInit, e);
        }

        void IPluginManager.RaiseServerActivate(int clientMax)
        {
            ServerActivateEventArgs e = new ServerActivateEventArgs()
            {
                ClientMax = clientMax,
            };
            this.Raise(this.m_engine.RaiseServerActivate, e);
        }

        void IPluginManager.RaiseLevelShutdown()
        {
            EventArgs e = new EventArgs();
            this.Raise(this.m_engine.RaiseLevelShutdown, e);
        }

        void IPluginManager.RaiseClientActive()
        {
            EventArgs e = new EventArgs();
            this.Raise(this.m_engine.RaiseClientActive, e);
        }

        void IPluginManager.RaiseClientDisconnect()
        {
            EventArgs e = new EventArgs();
            this.Raise(this.m_engine.RaiseClientDisconnect, e);
        }

        void IPluginManager.RaiseClientPutInServer()
        {
            EventArgs e = new EventArgs();
            this.Raise(this.m_engine.RaiseClientPutInServer, e);
        }

        void IPluginManager.RaiseClientSettingsChanged()
        {
            EventArgs e = new EventArgs();
            this.Raise(this.m_engine.RaiseClientSettingsChanged, e);
        }

        void IPluginManager.RaiseClientConnect()
        {
            EventArgs e = new EventArgs();
            this.Raise(this.m_engine.RaiseClientConnect, e);
        }

        void IPluginManager.RaiseClientCommand()
        {
            EventArgs e = new EventArgs();
            this.Raise(this.m_engine.RaiseClientCommand, e);
        }

        void IPluginManager.RaiseGameEvent(Int64 evtDataPtr, int evtArgsCount, Int64 evtArgsPtr)
        {
            //NativeEventData evtData = new NativeEventData();
            //Marshal.PtrToStructure(new IntPtr(evtDataPtr), evtData);
            NativeEventData evtData = (NativeEventData)Marshal.PtrToStructure(new IntPtr(evtDataPtr), typeof(NativeEventData));
            NativeEventArgs[] args = new NativeEventArgs[evtArgsCount];
            int size = Marshal.SizeOf(typeof(NativeEventArgs));
            for (int i = 0; i < evtArgsCount; i++)
            {
                IntPtr argPtr = new IntPtr(evtArgsPtr + i * size);
                args[i] = (NativeEventArgs)Marshal.PtrToStructure(argPtr, typeof(NativeEventArgs));
            }
            GameEventEventArgs e = new GameEventEventArgs()
            {
                Event = evtData.Event,
                Args = args.Select(a => new GameEventArgument(a)).ToArray(),
            };
            this.Raise(this.m_engine.RaiseGameEvent, e);
        }
    }
}
