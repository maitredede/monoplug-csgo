using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace MonoPlug
{
    internal sealed class ClsRemote : MarshalByRefObject
    {
        private readonly AppDomain _current;

        public string AppDomainName { get { return this._current.FriendlyName; } }

        public ClsRemote()
        {
            this._current = AppDomain.CurrentDomain;
        }

        public ClsPluginBase CreatePluginClass(IMessage msg, string assemblyBaseDir, PluginDefinition plugin)
        {
            return (ClsPluginBase)CreateInDomain(this._current, msg, Path.Combine(assemblyBaseDir, plugin.File), plugin.Type);
        }

        public static T CreateInDomain<T>(AppDomain domain, IMessage msg) where T : MarshalByRefObject
        {
            return CreateInDomain<T>(domain, msg, typeof(T).FullName);
        }

        public static T CreateInDomain<T>(AppDomain domain, IMessage msg, string typeName) where T : MarshalByRefObject
        {
            return (T)CreateInDomain(domain, msg, typeof(T).Assembly.Location, typeName);
        }
        public static object CreateInDomain(AppDomain domain, IMessage msg, string assemblyFile, string typeName)
        {
#if DEBUG
            msg.DevMsg("Entering Remote::CreateInDomain in [{0}] for [{1}]...\n", AppDomain.CurrentDomain.FriendlyName, domain.FriendlyName);
            try
            {
                msg.DevMsg("  Calling {0}.LoadFile()\n", typeof(Assembly).FullName);
#endif
                Assembly remoteSystemAssemnly = domain.Load(typeof(Assembly).Assembly.FullName);
                Type remoteAssemblyType = remoteSystemAssemnly.GetType(typeof(Assembly).FullName);
                Assembly remoteAssembly = (Assembly)remoteAssemblyType.InvokeMember("LoadFile", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { assemblyFile });

#if DEBUG
                msg.DevMsg("  Creating object instance...\n");
#endif
                object item = domain.CreateInstanceAndUnwrap(remoteAssembly.FullName, typeName);

#if DEBUG
                msg.DevMsg("  Creating object instance OK !\n");
#endif
                return item;
#if DEBUG
            }
            finally
            {
                msg.DevMsg("Exiting Remote::CreateInDomain...\n");
            }
#endif
        }

        public PluginDefinition[] GetPluginsFromDirectory(IMessage msg, string path)
        {
#if DEBUG
            msg.DevMsg("GetPluginsFromDirectory: Scanning path {0}\n", path);
            try
            {
#endif
                List<PluginDefinition> lst = new List<PluginDefinition>();
                string[] files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
#if DEBUG
                msg.DevMsg("   Files count : {0}\n", files.Length);
#endif
                foreach (string file in files)
                {
#if DEBUG
                    msg.DevMsg("   Scanning file {0}\n", file);
#endif
                    try
                    {
                        string filename = Path.Combine(path, file);
#if DEBUG
                        msg.DevMsg("   Filename is  {0}\n", filename);
#endif
                        Assembly asm = Assembly.LoadFile(filename);
                        foreach (Type t in asm.GetTypes())
                        {
                            try
                            {
                                if (!t.IsAbstract && t.IsSubclassOf(typeof(ClsPluginBase)) && t.IsPublic)
                                {
#if DEBUG
                                    msg.DevMsg("   Type is ClsPluginBase {0}\n", t.FullName);
#endif
                                    ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                                    if (ctor != null)
                                    {
                                        ClsPluginBase plugin = (ClsPluginBase)ctor.Invoke(null);
                                        PluginDefinition definition = new PluginDefinition();
                                        definition.File = Path.GetFileName(filename);
                                        definition.Name = plugin.Name;
                                        definition.Type = plugin.GetType().FullName;
                                        definition.Description = plugin.Description;
#if DEBUG
                                        msg.DevMsg("   Found Plugin {0}\n", plugin.Name);
#endif
                                        lst.Add(definition);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                msg.Warning("Can't create type : {0}\n", t.FullName);
#if DEBUG
                                msg.Warning(ex);
#endif
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        msg.Warning("Can't load file : {0}\n", file);
#if DEBUG
                        msg.Warning(ex);
#endif
                    }
                }
                return lst.ToArray();
#if DEBUG
            }
            finally
            {
                msg.DevMsg("GetPluginsFromDirectory: exit\n");
            }
#endif
        }

#if DEBUG
        internal static void DumpDomainAssemblies(IMessage msg)
        {
            Assembly[] arr = AppDomain.CurrentDomain.GetAssemblies();
            msg.DevMsg("DumpCurrentDomainAssemblies : {0} loaded\n", arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                msg.DevMsg("  {0} :: {1}\n", arr[i].FullName, arr[i].Location);
            }
            msg.DevMsg("DumpCurrentDomainAssemblies : End\n");
        }
#endif
    }
}
