using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    partial class ClsMain
    {
        void IEngineWrapper.ClientDialogMessage(ClsPlayer player, string title, string message, Color color, int level, int time)
        {
            DialogMessage msg = new DialogMessage();
            msg.Client = player;
            msg.Title = title;
            msg.Message = message;
            msg.Color = color;
            msg.Level = level;
            msg.Time = time;
            this.InterThreadCall<object, DialogMessage>(this.ClientDialogMessage_CALL, msg);
        }

        private object ClientDialogMessage_CALL(DialogMessage msg)
        {
            NativeMethods.Mono_ClientDialogMessage(msg.Client.UserId, msg.Title, msg.Message, msg.Color.A, msg.Color.R, msg.Color.G, msg.Color.B, msg.Level, msg.Time);
            return null;
        }

        void IEngineWrapper.ClientMenuMessage(ClsPlayer player, string title, string message, int level, int time)
        {
            DialogMessage msg = new DialogMessage();
            msg.Client = player;
            msg.Title = title;
            msg.Message = message;
            msg.Level = level;
            msg.Time = time;
            this.InterThreadCall<object, DialogMessage>(this.ClientMenuMessage_CALL, msg);
        }

        private object ClientMenuMessage_CALL(DialogMessage msg)
        {
            NativeMethods.Mono_ClientDialogText(msg.Client.UserId, msg.Title, msg.Message, msg.Level, msg.Time);
            return null;
        }
    }
}
