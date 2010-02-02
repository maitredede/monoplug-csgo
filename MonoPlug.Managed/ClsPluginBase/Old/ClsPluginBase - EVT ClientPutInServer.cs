//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MonoPlug.Old
//{
//    partial class ClsPluginBase
//    {
//        private int _EventCounter_ClientPutInServer = 0;

//        /// <summary>
//        /// Event raised when a client has been put in server
//        /// </summary>
//        protected event EventHandler<ClientEventArgs> ClientPutInServer
//        {
//            add
//            {
//                lock (this._events)
//                {
//                    this._events.AddHandler(Events.ClientPutInServer, value);
//                    if (this._EventCounter_ClientPutInServer++ == 0)
//                    {
//                        this._engine.ClientPutInServer_Add(this);
//                    }
//                }
//            }
//            remove
//            {
//                lock (this._events)
//                {
//                    this._events.RemoveHandler(Events.ClientPutInServer, value);
//                    if (--this._EventCounter_ClientPutInServer == 0)
//                    {
//                        this._engine.ClientPutInServer_Remove(this);
//                    }
//                }
//            }
//        }

//        internal void Raise_ClientPutInServer(object sender, ClientEventArgs e)
//        {
//            EventHandler<ClientEventArgs> d;
//            lock (this._events)
//            {
//                d = (EventHandler<ClientEventArgs>)this._events[Events.ClientPutInServer];
//            }
//            if (d != null)
//            {
//                d.Invoke(sender, e);
//            }
//        }
//    }
//}
