using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace MonoPlug
{
    partial class ClsMain
    {
        /// <summary>
        /// Create a plugin instance 
        /// </summary>
        /// <param name="desc">
        /// A <see cref="PluginDesc"/> Description of plugin to create
        /// </param>
        /// <returns>
        /// A <see cref="ClsPluginBase"/> Plugin instance
        /// </returns>
        private ClsPluginBase Remote_CreatePlugin(ClsMain owner, PluginDefinition desc)
        {
            try
            {
                owner.Msg("RM: Trying to load file : {0}\n", desc.File);
                Assembly asm = Assembly.LoadFile(desc.File);
                owner.Msg("RM: Trying to get type : {0}\n", desc.Type);
                Type t = asm.GetType(desc.Type, true);
                owner.Msg("RM: Trying to call ctor\n");
                ClsPluginBase plugin = (ClsPluginBase)t.GetConstructor(Type.EmptyTypes).Invoke(null);
                owner.Msg("RM: Plugin created\n");
                return plugin;
            }
            catch (Exception ex)
            {
                owner.Msg("RM: Plugin error : {0}, {1}\n", ex.GetType().FullName, ex.Message);
                owner.Msg(ex.StackTrace);
                throw ex;
            }
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
                                ClsPluginBase plugin = (ClsPluginBase)ctor.Invoke(null);
                                PluginDefinition definition = new PluginDefinition();
                                definition.File = filename;
                                definition.Name = plugin.Name;
                                definition.Type = plugin.GetType().FullName;
                                definition.Description = plugin.Description;
                                lst.Add(definition);
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
