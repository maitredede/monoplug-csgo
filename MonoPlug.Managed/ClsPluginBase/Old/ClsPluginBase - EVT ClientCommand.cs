//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MonoPlug.Old
//{
//    partial class ClsPluginBase
//    {
//        //private int _EventCounter_ClientCommand = 0;
//        //private static readonly object _EventToken_ClientCommand = new object();

//        //protected event EventHandler<ClientCommandEventArgs> ClientCommand
//        //{
//        //    add
//        //    {
//        //        lock (this._events)
//        //        {
//        //            this._events.AddHandler(_EventToken_ClientCommand, value);
//        //            if (this._EventCounter_ClientCommand++ == 0)
//        //            {
//        //                this._main.ClientCommand_Add(this);
//        //            }
//        //        }
//        //    }
//        //    remove
//        //    {
//        //        lock (this._events)
//        //        {
//        //            this._events.RemoveHandler(_EventToken_ClientCommand, value);
//        //            if (--this._EventCounter_ClientCommand == 0)
//        //            {
//        //                this._main.ClientCommand_Remove(this);
//        //            }
//        //        }
//        //    }
//        //}

//        //internal void Raise_ClientCommand(object sender, ClientCommandEventArgs e)
//        //{
//        //    Msg("ClsPluginBase::Raise_ClientCommand BEGIN\n");
//        //    EventHandler<ClientCommandEventArgs> d;
//        //    lock (this._events)
//        //    {
//        //        d = (EventHandler<ClientCommandEventArgs>)this._events[_EventToken_ClientCommand];
//        //    }
//        //    if (d != null)
//        //    {
//        //        Msg("ClsPluginBase::Raise_ClientCommand INVOKE\n");
//        //        d.Invoke(sender, e);
//        //    }
//        //    Msg("ClsPluginBase::Raise_ClientCommand END\n");
//        //}
//    }
//}
