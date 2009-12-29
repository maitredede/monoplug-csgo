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
        private List<MessageEntry> _lstMsg = null;
        /// <summary>
        /// Plugin instanciated and running 
        /// </summary>
        private Dictionary<AppDomain, ClsPluginBase> _plugins = null;
        /// <summary>
        /// Available plugin cache list 
        /// </summary>
        private PluginDefinition[] _pluginCache = null;
        #endregion

        public ClsMain()
        {
        }

        /// <summary>
        /// Get plugins from main assembly directory, search in other appdomain
        /// </summary>
        /// <returns>
        /// A <see cref="PluginDesc"/> array of plugins found
        /// </returns>
        private PluginDefinition[] GetPlugins()
        {
            AppDomain dom = null;
            try
            {
                //Create another domain to gather plugin data
                dom = AppDomain.CreateDomain("GetPlugins");
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                Msg("CLR: Assembly path is : {0}\n", path);

                //Instanciate the remote wrapper
                ClsMain main = (ClsMain)dom.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, typeof(ClsMain).FullName);
                return main.Remote_GetPluginsFromDirectory(path);
            }
            catch (Exception ex)
            {
                Msg("GetPlugins Error : {0}\n", ex.Message);
                Msg("GetPlugins Error : {0}\n", ex.StackTrace);
                return new PluginDefinition[] { };
            }
            finally
            {
                if (dom != null)
                {
                    //Destroy remote domain
                    AppDomain.Unload(dom);
                }
            }
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
        private ClsPluginBase Remote_CreatePlugin(PluginDefinition desc)
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
        private PluginDefinition[] Remote_GetPluginsFromDirectory(string path)
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
                                ClsPluginBase plugin = (ClsPluginBase)ctor.Invoke(null);
                                PluginDefinition definition = new PluginDefinition();
                                definition.File = filename;
                                definition.Name = plugin.Name;
                                definition.Type = plugin.GetType().FullName;
                                lst.Add(definition);
                            }
                        }
                        catch//(Exception ex)
                        {
                            Msg("Can't create type : {0}\n", t.FullName);
                        }
                    }
                }
                catch //(Exception ex)
                {
                    Msg("Can't load file : {0}\n", file);
                }
            }
            return lst.ToArray();
        }
    }
}