using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class InternalSayCommand : ObjectBase
    {
        private readonly string _name;
        private readonly bool _async;
        private readonly bool _hidden;
        private readonly ClsPluginBase _plugin;
        private readonly ConCommandDelegate _code;
        private readonly IThreadPool _thpool;

        internal InternalSayCommand(string name, bool async, bool hidden, ClsPluginBase plugin, ConCommandDelegate code, IThreadPool pool)
        {
            Check.NonNull("pool", pool);

            this._name = name;
            this._async = async;
            this._hidden = hidden;
            this._plugin = plugin;
            this._code = code;
            this._thpool = pool;
        }

        public string Name { get { return this._name; } }
        public ClsPluginBase Plugin { get { return this._plugin; } }

        public void Execute(ClsPlayer sender, string line, string[] args)
        {
            if (this._async)
            {
                this._thpool.QueueUserWorkItem(this.DoExec, sender, line, args);
            }
            else
            {
                this.DoExec(sender, line, args);
            }
        }

        private void DoExec(ClsPlayer sender, string line, string[] args)
        {
            this._code(sender, line, args);
        }
    }
}
