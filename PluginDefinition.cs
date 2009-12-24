
using System;

namespace MonoPlug
{
	internal sealed class PluginDefinition
	{
		public string File;
		public string Type;
		public string Name;
		
		public PluginDefinition Clone()
		{
			return (PluginDefinition)this.MemberwiseClone();
		}
	}
}
