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
            AppDomain dom = AppDomain.CreateDomain(name);
#if DEBUG
            this.Msg("DBG: Created domain {0}\n", name);
#endif
#if DEBUG
            this.Msg("DBG: Creating proxy for domain {0}\n", name);
#endif
            proxy = (ClsMain)dom.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ClsMain).FullName);
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
