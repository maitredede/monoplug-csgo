using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsMain
    {
        private static readonly object EventConMessage = new object();

        internal void ConMessage_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, EventConMessage, NativeMethods.Mono_AttachConsole);
        }

        internal void ConMessage_Remove(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, EventConMessage, NativeMethods.Mono_DetachConsole);
        }

        internal void Raise_ConMessage(bool hasColor, bool debug, int r, int g, int b, int a, string msg)
        {
            ConMessageEventArgs e = new ConMessageEventArgs(hasColor, debug, Color.FromArgb(a, r, g, b), msg);
            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(EventConMessage))
                {
                    lst = new List<ClsPluginBase>(this._events[EventConMessage]);
                }
            }
            if (lst != null && lst.Count > 0)
            {
                foreach (ClsPluginBase plugin in lst)
                {
                    plugin.Raise_ConMessage(this, e);
                }
            }
        }
    }
}
