using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsPluginMessage : ObjectBase, IMessage
    {
        private readonly IMessage _msg;
        private readonly ClsPluginBase _plugin;

        internal ClsPluginMessage(ClsPluginBase owner, IMessage msg)
        {
            this._msg = msg;
            this._plugin = owner;
        }

        public void Msg(string format, params object[] elements)
        {
            this._msg.Msg(format, elements);
        }

        public void DevMsg(string format, params object[] elements)
        {
            this._msg.DevMsg(format, elements);
        }

        public void Warning(string format, params object[] elements)
        {
            this._msg.Warning(format, elements);
        }

        public void Warning(Exception ex)
        {
            this._msg.Warning(ex);
        }

        public void Error(string format, params object[] elements)
        {
            this._msg.Error(format, elements);
        }

        public void Error(Exception ex)
        {
            this._msg.Error(ex);
        }

        public void Log(string format, params object[] elements)
        {
            if (elements != null && elements.Length > 0)
            {
                format = string.Format(format, elements);
            }
            this._msg.Error("[{0}] {1}", this._plugin.Name, format);
        }
    }
}
