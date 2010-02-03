using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAttach.ConMessage_Add(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ConsoleMessage, NativeMethods.Mono_AttachConsole);
        }

        void IEventsAttach.ConMessage_Remove(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ConsoleMessage, NativeMethods.Mono_DetachConsole);
        }

        internal void Raise_ConMessage(bool hasColor, bool debug, int r, int g, int b, int a, string msg)
        {
            //ConMessageEventArgs e = new ConMessageEventArgs(hasColor, debug, Color.FromArgb(a, r, g, b), msg);
            List<ClsPluginBase> lst = null;
            lock (this._events)
            {
                if (this._events.ContainsKey(Events.ConsoleMessage))
                {
                    lst = new List<ClsPluginBase>(this._events[Events.ConsoleMessage]);
                }
            }
            if (lst != null && lst.Count > 0)
            {
                foreach (ClsPluginBase plugin in lst)
                {
                    plugin.PluginEvents.Raise_ConMessage(hasColor, debug, r, g, b, a, msg);
                }
            }
        }
    }
}
