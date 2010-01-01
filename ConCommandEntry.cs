using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    internal struct ConCommandEntry
    {
		private ClsPluginBase _plugin;
		private ClsConCommand _command;
		
        public ClsPluginBase Plugin { get{return this._plugin;} set{this._plugin=value;} }
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public ConCommandDelegate Code { get; set; }
        public ClsConCommand Command { get{return this._command;} set{this._command=value;} }
    }
}
