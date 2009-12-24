
using System;
using System.Collections.Generic;

namespace MonoPlug
{
	internal struct ConCommandEntry
	{
		public ClsPluginBase Plugin;
		public string Name;
		public string Description;
		public ConCommandDelegate Code;
	}
	
	partial class ClsMain
	{
		private Dictionary<string, ConCommandEntry> _commands;
		
		internal bool RegisterCommand(ClsPluginBase plugin, string name, string description, ConCommandDelegate code)
		{
			lock(this._commands)
			{
				if(this._commands.ContainsKey(name))
				{
					return false;
				}
				
				ConCommandEntry entry = new ConCommandEntry();
				entry.Plugin = plugin;
				entry.Name=name;
				entry.Description=description;
				entry.Code=code;
				
				if(Mono_RegisterConCommand(name,description, code))
				{
					this._commands.Add(name, entry);
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
}
