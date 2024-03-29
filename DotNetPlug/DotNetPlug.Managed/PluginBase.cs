﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Base plugin class
    /// </summary>
#if !DEBUG
    [System.Diagnostics.DebuggerNonUserCode]
#endif
    public abstract class PluginBase : IPlugin
    {
        private Configuration m_config;

        /// <summary>
        /// Loads this instance.
        /// </summary>
        /// <returns></returns>
        public abstract Task Load();
        /// <summary>
        /// Unloads this instance.
        /// </summary>
        /// <returns></returns>
        public abstract Task Unload();

        private IEngine m_engine;

        void IPlugin.Init(IEngine engine)
        {
            this.m_engine = engine;
        }

        /// <summary>
        /// Source engine interface
        /// </summary>
        protected IEngine Engine { get { return this.m_engine; } }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Configuration GetConfig()
        {
            if (this.m_config == null)
            {
                this.m_config = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            }
            return this.m_config;
        }

        /// <summary>
        /// Reloads the configuration.
        /// </summary>
        public void ReloadConfig()
        {
            this.m_config = null;
            this.GetConfig();
        }
    }
}
