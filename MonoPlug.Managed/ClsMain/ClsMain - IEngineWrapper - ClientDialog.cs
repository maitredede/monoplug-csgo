using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEngineWrapper.ClientDialog(ClsPlayer player, string title, string message, Color color, int level, int time)
        {
            DialogMessage msg = new DialogMessage();
            msg.Client = player;
            msg.Title = title;
            msg.Message = message;
            msg.Color = color;
            msg.Level = level;
            msg.Time = time;
            this.InterThreadCall<object, DialogMessage>(this.ClientDialog_CALL, msg);
        }

        private object ClientDialog_CALL(DialogMessage msg)
        {
            NativeMethods.Mono_ClientDialogMessage(msg.Client.Id, msg.Title, msg.Message, msg.Color.A, msg.Color.R, msg.Color.G, msg.Color.B, msg.Level, msg.Time);
            return null;
        }
    }
}
