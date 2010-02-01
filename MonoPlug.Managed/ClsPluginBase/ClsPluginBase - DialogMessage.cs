//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Drawing;

//namespace MonoPlug
//{
//    partial class ClsPluginBase
//    {
//        /// <summary>
//        /// Send a DialogMessage to the client
//        /// </summary>
//        /// <param name="client">Target client</param>
//        /// <param name="title">Dialog title</param>
//        /// <param name="message">Dialog message</param>
//        /// <param name="color">Text color</param>
//        /// <param name="level">Text level</param>
//        /// <param name="time">Duration or message</param>
//        protected void ClientDialogMessage(ClsPlayer client, string title, string message, Color color, int level, int time)
//        {
//            Check.NonNull("client", client);
//            Check.NonNullOrEmpty("title", title);
//            Check.NonNullOrEmpty("message", message);
//            Check.InRange("level", level, 0, 10);
//            Check.InRange("time", time, 0, 10);

//            DialogMessage msg = new DialogMessage();
//            msg.Client = client;
//            msg.Title = title;
//            msg.Message = message;
//            msg.Color = color;
//            msg.Level = level;
//            msg.Time = time;

//            this._main.InterThreadCall<object, DialogMessage>(this.ClientDialogMessage_CALL, msg);
//        }

//        private object ClientDialogMessage_CALL(DialogMessage msg)
//        {
//            NativeMethods.Mono_ClientDialogMessage(msg.Client.Id, msg.Title, msg.Message, msg.Color.A, msg.Color.R, msg.Color.G, msg.Color.B, msg.Level, msg.Time);
//            return null;
//        }
//    }
//}
