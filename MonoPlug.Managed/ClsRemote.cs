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
                                    definition.File = Path.GetFileName(filename);
                                    definition.Name = plugin.Name;
                                    definition.Type = plugin.GetType().FullName;
                                    definition.Description = plugin.Description;
                                    lst.Add(definition);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            msg.Error("Can't create type : {0}\n", t.FullName);
#if DEBUG
                            msg.Error(ex);
#endif
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg.Warning("Can't load file : {0}\n", file);
#if DEBUG
                    msg.Error(ex);
#endif
                }
            }
            return lst.ToArray();
        }

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
    }
}
