using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MonoPlug
{
    /// <summary>
    /// Main class that handle everything
    /// </summary>
    internal sealed partial class ClsMain : MarshalByRefObject
    {
        #region Private fields
        /// <summary>
        /// List of awaiting Messages 
        /// </summary>
        private List<MessageEntry> _lstMsg;
        /// <summary>
        /// Plugin instanciated and running 
        /// </summary>
        private Dictionary<AppDomain, ClsPluginBase> _plugins;
        /// <summary>
        /// Available plugin cache list 
        /// </summary>
        private PluginDefinition[] _pluginCache;
        #endregion

        public ClsMain()
        {
        }

        /// <summary>
        /// Callback for plugin load 
        /// </summary>
        /// <param name="type">
        /// The <see cref="System.String"/> that represent the plugin type (shown in clr_plugin_list)
        /// </param>
        internal void _PluginLoad(string type)
        {
            try
            {
                lock (this._plugins)
                {
                    PluginDefinition desc = null;
                    foreach (PluginDefinition plug in this._pluginCache)
                    {
                        if (plug.Type == type)
                        {
                            desc = plug;
                            break;
                        }
                    }

                    if (desc == null)
                    {
                        Mono_Msg("Can't find plugin type : " + type + "\n");
                    }
                    else
                    {
                        try
                        {
                            AppDomain dom = AppDomain.CreateDomain(desc.Name);
                            ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, typeof(ClsMain).FullName);
                            ClsPluginBase plugin = main.CreatePlugin(desc);
                            this._plugins.Add(dom, plugin); ;
                            plugin.Init(this);
                        }
                        catch (Exception ex)
                        {
                            Mono_Msg("Can't load plugin : " + type + "\n");
                            Mono_Msg(ex.Message + "\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mono_Msg("PluginLoad Exception : " + ex.GetType().FullName + "\n");
                Mono_Msg(ex.Message + "\n");
                Mono_Msg(ex.StackTrace + "\n");
            }
        }

        private void AppendMessage(MessageType t, string m)
        {
            if (!string.IsNullOrEmpty(m))
            {
                lock (this._lstMsg)
                {
                    this._lstMsg.Add(new MessageEntry(t, m));
                }
            }
        }

        /// <summary>
        /// Enqueue a Msg 
        /// </summary>
        /// <param name="msg">
        /// A <see cref="System.String"/> : The message to enqueue
        /// </param>
        internal void Msg(string msg)
        {
            this.AppendMessage(MessageType.Msg, msg);
        }

        /// <summary>
        /// Get plugins from main assembly directory, search in other appdomain
        /// </summary>
        /// <returns>
        /// A <see cref="PluginDesc"/> array of plugins found
        /// </returns>
        private PluginDefinition[] GetPlugins()
        {
            List<PluginDefinition> lst = new List<PluginDefinition>();
            try
            {
                AppDomain dom = AppDomain.CreateDomain("GetPlugins");
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Mono_Msg("Path is :" + path + "\n");
                ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, typeof(ClsMain).FullName);
                //Assembly asm = dom.Load(Assembly.GetExecutingAssembly().Location);
                //Type t = asm.GetType(typeof(ClsMain).FullName, true);
                ClsPluginBase[] arr = main.GetPluginsFromDirectory(path);

                foreach (ClsPluginBase plugin in arr)
                {
                    PluginDefinition desc = new PluginDefinition();
                    desc.File = plugin.GetType().Assembly.Location;
                    desc.Name = plugin.Name;
                    desc.Type = plugin.GetType().FullName;
                    lst.Add(desc);
                }

                AppDomain.Unload(dom);
            }
            catch (Exception ex)
            {
                Mono_Msg(string.Format("GetPlugins Error : {0}\n", ex.Message));
                Mono_Msg(string.Format("GetPlugins Error : {0}\n", ex.StackTrace));
            }
            return lst.ToArray();
        }

        /// <summary>
        /// Create a plugin instance 
        /// </summary>
        /// <param name="desc">
        /// A <see cref="PluginDesc"/> Description of plugin to create
        /// </param>
        /// <returns>
        /// A <see cref="ClsPluginBase"/> Plugin instance
        /// </returns>
        private ClsPluginBase CreatePlugin(PluginDefinition desc)
        {
            Assembly asm = Assembly.LoadFile(desc.File);
            Type t = asm.GetType(desc.Type, true);
            ClsPluginBase plugin = (ClsPluginBase)t.GetConstructor(Type.EmptyTypes).Invoke(null);
            return plugin;
        }

        /// <summary>
        /// Get plugins of a directory 
        /// </summary>
        /// <param name="path">
        /// A <see cref="System.String"/> Path to search
        /// </param>
        /// <returns>
        /// A <see cref="ClsPluginBase"/> Plugins array
        /// </returns>
        private ClsPluginBase[] GetPluginsFromDirectory(string path)
        {
            List<ClsPluginBase> lst = new List<ClsPluginBase>();
            string[] files = Directory.GetFiles(path, "*.dll");
            foreach (string file in files)
            {

                try
                {
                    Assembly asm = Assembly.LoadFile(Path.Combine(path, file));
                    foreach (Type t in asm.GetTypes())
                    {
                        try
                        {
                            if (!t.IsAbstract && t.IsSubclassOf(typeof(ClsPluginBase)) && t.IsPublic)
                            {
                                ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                                ClsPluginBase plugin = (ClsPluginBase)ctor.Invoke(null);
                                lst.Add(plugin);
                            }
                        }
                        catch//(Exception ex)
                        {
                            Mono_Msg(string.Format("Can't create type : {0}\n", t.FullName));
                        }
                    }
                }
                catch //(Exception ex)
                {
                    Mono_Msg(string.Format("Can't load file : {0}\n", file));
                }
            }
            return lst.ToArray();
        }

        /// <value>
        /// Get Mono Runtime version
        /// </value>
        internal static string MonoVersion
        {
            get
            {
                Type t = Type.GetType("Mono.Runtime");
                if (t == null)
                    return "Not on Mono Runtime";
                PropertyInfo prop = t.GetProperty("Version");
                if (prop == null)
                {
                    t = Type.GetType("Consts", false, false);
                    if (t == null)
                    {
                        return ("Can't Mono.Runtime.Version or Consts.MonoVersion");
                    }
                    else
                    {
                        FieldInfo f = t.GetField("MonoVersion");
                        return (string)f.GetValue(null);
                    }
                }
                else
                {
                    object version = prop.GetValue(null, null);
                    if (version == null)
                    {
                        return "Version is null";
                    }
                    return prop.GetValue(null, null).ToString();
                }
            }
        }
    }
}

