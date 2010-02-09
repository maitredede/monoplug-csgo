using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace MonoPlug
{
    internal sealed class ClsProxy : ObjectBase
    {
        private readonly AppDomain _current;

        /// <summary>
        /// Name of AppDomain
        /// </summary>
        public string AppDomainName { get { return this._current.FriendlyName; } }

        internal static ClsProxy CreateProxy(AppDomain target, IMessage msg)
        {
            //Load the System assembly
            Assembly remoteSystemAssemnly = target.Load(typeof(Assembly).Assembly.FullName);
            //Get the "Assembly" type
            Type remoteAssemblyType = remoteSystemAssemnly.GetType(typeof(Assembly).FullName);
            //call the Assenmnly.Load function to load assembly containing proxy class
            Assembly remoteAssembly = (Assembly)remoteAssemblyType.InvokeMember("LoadFile",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                null, null,
                new object[] { Assembly.GetExecutingAssembly().Location });
            Type remoteProxyType = remoteAssembly.GetType(typeof(ClsProxy).FullName);

            ClsProxy proxy = (ClsProxy)target.CreateInstanceAndUnwrap(remoteAssembly.FullName, remoteProxyType.FullName);

            return proxy;
        }

        public ClsProxy()
        {
            this._current = AppDomain.CurrentDomain;
#if DEBUG
            NativeMethods.Mono_DevMsg(string.Format("New Proxy in domain [{0}]\n", this._current.FriendlyName));
#endif
        }

        /// <summary>
        /// Create a plugin in proxied domain
        /// </summary>
        /// <param name="msg">Logger</param>
        /// <param name="assemblyBaseDir">Base directory for loading assembly</param>
        /// <param name="plugin">Plugin definition</param>
        /// <returns>Plugin instance</returns>
        public ClsPluginBase CreatePluginClass(IMessage msg, string assemblyBaseDir, PluginDefinition plugin)
        {
            Assembly asm = Assembly.LoadFile(Path.Combine(assemblyBaseDir, plugin.File));
            Type t = asm.GetType(plugin.Type);
            return (ClsPluginBase)Activator.CreateInstance(t);
        }

        public IMessage CreatePluginMessage(ClsPluginBase owner, IMessage msg)
        {
            return new ClsPluginMessage(owner, msg);
        }

        public IEngine CreatePluginConItem(ClsPluginBase owner, ClsMain main)
        {
            return new ClsPluginEngine(owner, main);
        }

        public PluginDefinition[] GetPluginsFromDirectory(IMessage msg, string path)
        {
            List<PluginDefinition> lst = new List<PluginDefinition>();
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
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
                            msg.Warning("Can't create type : '{0}' : {1}\n", t.FullName, ex.Message);
                        }
                    }
                }
                catch (BadImageFormatException)
                {
                    msg.Warning("Can't load file : '{0}', not a valid assembly\n", file);
                }
                catch (Exception ex)
                {
                    msg.Warning("Can't load file : '{0}', {1}\n", file, ex.Message);
                }
            }
            return lst.ToArray();
        }

#if DEBUG
        internal void DumpDomainAssemblies(IMessage msg)
        {
            Assembly[] arr = this._current.GetAssemblies();
            msg.DevMsg("DumpCurrentDomainAssemblies : {0} loaded\n", arr.Length);
            for (int i = 0; i < arr.Length; i++)
            {
                msg.DevMsg("  {0} :: {1}\n", arr[i].FullName, arr[i].Location);
            }
            msg.DevMsg("DumpCurrentDomainAssemblies : End\n");
        }
#endif

        internal static void WriteAssemblyVersion(IMessage msg, Type type, string format)
        {
            try
            {
                AssemblyName asmName = type.Assembly.GetName();
                if (asmName != null)
                {
                    msg.Msg(format, asmName.Version.ToString(4));
                }
                else
                {
                    object[] arr = type.Assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), true);
                    if (arr != null && arr.Length > 0)
                    {
                        foreach (AssemblyVersionAttribute att in arr)
                        {
                            msg.Msg(format, att.Version);
                        }
                    }
                    else
                    {
                        msg.Msg(format, "no version");
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Warning(format, ex.Message);
            }
        }
    }
}
