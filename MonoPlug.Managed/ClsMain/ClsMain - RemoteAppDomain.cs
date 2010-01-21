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

        private AppDomain CreateAppDomain(string name, out ClsMain proxy)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = this.GetAssemblyDirectory();
            //setup.ShadowCopyFiles = true.ToString();

#if DEBUG
            this.Msg("DBG: Creatin domain {0}\n", name);
#endif
            AppDomain dom = AppDomain.CreateDomain(name, null, setup);
#if DEBUG
            this.Msg("DBG: Get assembly mscore for domain {0}\n", name);
#endif
            Assembly system = dom.Load(typeof(Assembly).Assembly.FullName);
#if DEBUG
            this.Msg("DBG: Get System.Reflection.Assembly type for domain {0}\n", name);
#endif
            Type asmType = system.GetType(typeof(Assembly).FullName);
#if DEBUG
            this.Msg("DBG: Loading MonoPlug main assembly in domain {0}\n", name);
#endif
            Assembly remoteMain = (Assembly)asmType.InvokeMember("LoadFile", BindingFlags.Public | BindingFlags.Static, null, null, new object[] { Assembly.GetExecutingAssembly().Location });
#if DEBUG
            this.Msg("DBG: Creating proxy for domain {0}\n", name);
#endif
            proxy = (ClsMain)dom.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(ClsMain).FullName);
#if DEBUG
            this.Msg("DBG: Created proxy for domain {0}\n", name);
#endif
            return dom;
        }

        //private ClsMain _remote_owner;

        private ClsPluginBase Remote_CreatePlugin(ClsMain owner, PluginDefinition desc)
        {
            //this._remote_owner = owner;
            try
            {
                this.Msg("DBG:RM: Loading file {0}\n", desc.File);
                Assembly asm = Assembly.LoadFile(desc.File);
#if DEBUG
                DumpCurrentDomainAssemblies(owner);
#endif
                Type t = asm.GetType(desc.Type, true);
                ClsPluginBase plugin = (ClsPluginBase)t.GetConstructor(Type.EmptyTypes).Invoke(null);
                return plugin;
            }
            catch (Exception ex)
            {
                owner.Msg("RM: Plugin error : {0}, {1}\n", ex.GetType().FullName, ex.Message);
                owner.Msg(ex.StackTrace);
                throw ex;
            }
        }

        //private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        //{
        //    Console.WriteLine("RM: AssemblyLoad({0}) {1}", AppDomain.CurrentDomain.FriendlyName, args.LoadedAssembly.FullName);
        //}

        //private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    Console.WriteLine("RM: AssemblyResolve({0}) {1}", AppDomain.CurrentDomain.FriendlyName, args.Name);
        //    return Assembly.Load(args.Name);
        //}

        internal static void DumpCurrentDomainAssemblies(ClsMain main)
        {
            Assembly[] arr = AppDomain.CurrentDomain.GetAssemblies();
            main.Msg("DumpCurrentDomainAssemblies : {0} loaded\n", arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                main.Msg("  {0} :: {1}\n", arr[i].FullName, arr[i].Location);
            }
            main.Msg("DumpCurrentDomainAssemblies : End\n");
        }

        private PluginDefinition[] Remote_GetPluginsFromDirectory(ClsMain owner, string path)
        {
            List<PluginDefinition> lst = new List<PluginDefinition>();
            string[] files = Directory.GetFiles(path, "*.dll");
            foreach (string file in files)
            {

                try
                {
                    string filename = Path.Combine(path, file);
                    Assembly asm = Assembly.LoadFile(filename);
                    foreach (Type t in asm.GetTypes())
                    {
                        try
                        {
                            if (!t.IsAbstract && t.IsSubclassOf(typeof(ClsPluginBase)) && t.IsPublic)
                            {
                                ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                                if (ctor != null)
                                {
                                    ClsPluginBase plugin = (ClsPluginBase)ctor.Invoke(null);
                                    PluginDefinition definition = new PluginDefinition();
                                    definition.File = filename;
                                    definition.Name = plugin.Name;
                                    definition.Type = plugin.GetType().FullName;
                                    definition.Description = plugin.Description;
                                    lst.Add(definition);
                                }
                            }
                        }
                        catch//(Exception ex)
                        {
                            owner.Msg("Can't create type : {0}\n", t.FullName);
                        }
                    }
                }
                catch //(Exception ex)
                {
                    owner.Msg("Can't load file : {0}\n", file);
                }
            }
            return lst.ToArray();
        }
    }
}
