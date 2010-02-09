using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEventsAnchor.ConsoleMessage_Attach(ClsPluginBase plugin)
        {
            this.Event_Add(plugin, Events.ConsoleMessage, NativeMethods.Mono_AttachConsole);
        }

        void IEventsAnchor.ConsoleMessage_Detach(ClsPluginBase plugin)
        {
            this.Event_Remove(plugin, Events.ConsoleMessage, NativeMethods.Mono_DetachConsole);
        }

        internal void Raise_ConMessage(bool hasColor, bool debug, int r, int g, int b, int a, string msg)
        {
            foreach (ClsPluginBase plugin in this.GetHandlerPlugins(Events.ConsoleMessage))
            {
                plugin.PluginEvents.Raise_ConMessage(hasColor, debug, r, g, b, a, msg);
            }
        }
    }
}
