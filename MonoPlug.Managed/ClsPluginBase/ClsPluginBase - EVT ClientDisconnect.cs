using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginBase
    {
        //private int _EventCounter_ClientDisconnect = 0;
        //private static readonly object _EventToken_ClientDisconnect = new object();

        /// <summary>
        /// Event raised when a client has been put in server
        /// </summary>
        protected event EventHandler<ClientEventArgs> ClientDisconnect
        {
            add
            {
                lock (this._events)
                {
                    this._events.AddHandler(Events.ClientDisconnect, value);
                    //if (this._EventCounter_ClientDisconnect++ == 0)
                    //{
                    //    this._main.ClientDisconnect_Add(this);
                    //}
                }
            }
            remove
            {
                lock (this._events)
                {
                    this._events.RemoveHandler(Events.ClientDisconnect, value);
                    //if (--this._EventCounter_ClientDisconnect == 0)
                    //{
                    //    this._main.ClientDisconnect_Remove(this);
                    //}
                }
            }
        }

        internal void Raise_ClientDisconnect(object sender, ClientEventArgs e)
        {
            EventHandler<ClientEventArgs> d;
            lock (this._events)
            {
                d = (EventHandler<ClientEventArgs>)this._events[Events.ClientDisconnect];
            }
            if (d != null)
            {
                d.Invoke(sender, e);
            }
        }
    }
}
