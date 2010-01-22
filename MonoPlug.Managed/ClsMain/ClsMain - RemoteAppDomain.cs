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

        //        private T RemoteCreateClass<T>(ClsMain owner, AppDomain dom) where T : class
        //        {
        //            return this.RemoteCreateClass<T>(owner, dom, typeof(T).FullName);
        //        }
        //        private T RemoteCreateClass<T>(ClsMain owner, AppDomain dom, string typeName) where T : class
        //        {
        //#if DEBUG
        //            owner.DevMsg("Entering RemoteCreateClass in [{0}] for [{1}]...\n", AppDomain.CurrentDomain.FriendlyName, dom.FriendlyName);
        //            try
        //            {
        //#endif
        //                Assembly remoteSystemAssemnly = dom.Load(typeof(Assembly).Assembly.FullName);
        //#if DEBUG
        //                owner.DevMsg("  RemoteCreateClass A in [{0}] for [{1}]...\n", AppDomain.CurrentDomain.FriendlyName, dom.FriendlyName);
        //#endif
        //                Type remoteAssemblyType = remoteSystemAssemnly.GetType(typeof(Assembly).FullName);
        //#if DEBUG
        //                owner.DevMsg("  RemoteCreateClass B in [{0}] for [{1}]...\n", AppDomain.CurrentDomain.FriendlyName, dom.FriendlyName);
        //#endif
        //                Assembly remoteAssembly = (Assembly)remoteAssemblyType.InvokeMember("LoadFile", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { typeof(T).Assembly.Location });
        //#if DEBUG
        //                owner.DevMsg("  RemoteCreateClass C in [{0}] for [{1}]...\n", AppDomain.CurrentDomain.FriendlyName, dom.FriendlyName);
        //#endif
        //                T item = (T)dom.CreateInstanceAndUnwrap(remoteAssembly.FullName, typeName);

        //#if DEBUG
        //                owner.DevMsg("  RemoteCreateClass OK in [{0}] for [{1}]...\n", AppDomain.CurrentDomain.FriendlyName, dom.FriendlyName);
        //#endif
        //                return item;
        //#if DEBUG
        //            }
        //            finally
        //            {
        //                owner.DevMsg("Exiting RemoteCreateClass...\n");
        //            }
        //#endif
        //        }

        //        internal static void DumpCurrentDomainAssemblies(ClsMain main)
        //        {
        //            Assembly[] arr = AppDomain.CurrentDomain.GetAssemblies();
        //            main.DevMsg("DumpCurrentDomainAssemblies : {0} loaded\n", arr.Length);
        //            for (int i = 0; i < arr.Length; i++)
        //            {
        //                main.DevMsg("  {0} :: {1}\n", arr[i].FullName, arr[i].Location);
        //            }
        //            main.DevMsg("DumpCurrentDomainAssemblies : End\n");
        //        }

        //        private PluginDefinition[] Remote_GetPluginsFromDirectory(ClsMain owner, string path)
        //        {
        //            List<PluginDefinition> lst = new List<PluginDefinition>();
        //            string[] files = Directory.GetFiles(path, "*.dll");
        //            foreach (string file in files)
        //            {

        //                try
        //                {
        //                    string filename = Path.Combine(path, file);
        //                    Assembly asm = Assembly.LoadFile(filename);
        //                    foreach (Type t in asm.GetTypes())
        //                    {
        //                        try
        //                        {
        //                            if (!t.IsAbstract && t.IsSubclassOf(typeof(ClsPluginBase)) && t.IsPublic)
        //                            {
        //                                ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
        //                                if (ctor != null)
        //                                {
        //                                    ClsPluginBase plugin = (ClsPluginBase)ctor.Invoke(null);
        //                                    PluginDefinition definition = new PluginDefinition();
        //                                    definition.File = Path.GetFileName(filename);
        //                                    definition.Name = plugin.Name;
        //                                    definition.Type = plugin.GetType().FullName;
        //                                    definition.Description = plugin.Description;
        //                                    lst.Add(definition);
        //                                }
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            owner.Msg("Can't create type : {0}\n", t.FullName);
        //#if DEBUG
        //                            owner.Msg(ex);
        //#endif
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    owner.Msg("Can't load file : {0}\n", file);
        //#if DEBUG
        //                    owner.Msg(ex);
        //#endif
        //                }
        //            }
        //            return lst.ToArray();
        //        }
    }
}
