using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsPluginEvents
    {
        internal void Raise_ConMessage(bool hasColor, bool debug, int r, int g, int b, int a, string msg)
        {
            this.Raise(Events.ConsoleMessage, this, new ConMessageEventArgs(hasColor, debug, Color.FromArgb(a, r, g, b), msg));
        }

        event EventHandler<ConMessageEventArgs> IEvents.ConsoleMessage
        {
            add { this.Attach(Events.ConsoleMessage, value, this._anchor.ConMessage_Add); }
            remove { this.Detach(Events.ConsoleMessage, value, this._anchor.ConMessage_Remove); }
        }
    }
}
