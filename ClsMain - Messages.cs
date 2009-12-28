using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain
    {
        /// <summary>
        /// Message handling function 
        /// </summary>		
        internal void _HandleMessages()
        {
            lock (this._lstMsg)
            {
                if (this._lstMsg.Count > 0)
                {
                    for (int i = 0; i < this._lstMsg.Count; i++)
                    {
                        MessageEntry msg = this._lstMsg[i];
                        Mono_Msg(msg.Message);
                    }
                    this._lstMsg.Clear();
                }
            }
        }
    }
}
