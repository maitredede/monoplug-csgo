//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MonoPlug.Old
//{
//    partial class ClsPluginBase
//    {
//        private int _EventCounter_ConMessage = 0;
//        //private static readonly object _EventToken_ConMessage = new object();

//        /// <summary>
//        /// Event raised when a console message has been printed
//        /// </summary>
//        protected event EventHandler<ConMessageEventArgs> ConMessage
//        {
//            add
//            {
//                lock (this._events)
//                {
//                    this._events.AddHandler(Events.ConsoleMessage, value);
//                    if (this._EventCounter_ConMessage++ == 0)
//                    {
//                        this._engine.ConMessage_Add(this);
//                    }
//                }
//            }
//            remove
//            {
//                lock (this._events)
//                {
//                    this._events.RemoveHandler(Events.ConsoleMessage, value);
//                    if (--this._EventCounter_ConMessage == 0)
//                    {
//                        this._engine.ConMessage_Remove(this);
//                    }
//                }
//            }

//        }

//        internal void Raise_ConMessage(object sender, ConMessageEventArgs e)
//        {
//            EventHandler d;
//            lock (this._events)
//            {
//                d = (EventHandler)this._events[Events.ConsoleMessage];
//            }
//            if (d != null)
//            {
//                d.Invoke(sender, e);
//            }
//        }
//    }
//}
