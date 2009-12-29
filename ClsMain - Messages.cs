using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        ///// <summary>
        ///// Message callback handling function 
        ///// </summary>		
        //internal void _HandleMessages()
        //{
        //    lock (this._lstMsg)
        //    {
        //        if (this._lstMsg.Count > 0)
        //        {
        //            for (int i = 0; i < this._lstMsg.Count; i++)
        //            {
        //                MessageEntry msg = this._lstMsg[i];
        //                Mono_Msg(msg.Message);
        //            }
        //            this._lstMsg.Clear();
        //        }
        //    }
        //}

        //private void AppendMessage(MessageType t, string m)
        //{
        //    if (!string.IsNullOrEmpty(m))
        //    {
        //        lock (this._lstMsg)
        //        {
        //            this._lstMsg.Add(new MessageEntry(t, m));
        //        }
        //    }
        //}

        ///// <summary>
        ///// Enqueue a Msg 
        ///// </summary>
        ///// <param name="msg">
        ///// A <see cref="System.String"/> : The message to enqueue
        ///// </param>
        //internal void Msg(string msg)
        //{
        //    this.AppendMessage(MessageType.Msg, msg);
        //}
        internal void Msg(string message)
        {
        }

        internal void Msg(string format, params object[] vals)
        {
            this.Msg(string.Format(format, vals));
        }
    }
}
