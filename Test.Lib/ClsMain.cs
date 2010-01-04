using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Test
{
    public sealed class ClsMain : MarshalByRefObject
    {
        private static readonly object _evt_plop = new object();
        private readonly EventHandlerList _events = new EventHandlerList();
        //public event DD Plop;

        public ClsMain()
        {
            Console.WriteLine("New in domain " + AppDomain.CurrentDomain.FriendlyName);
        }

        public void Start(ClsMain owner)
        {
            Console.WriteLine("Start in domain " + AppDomain.CurrentDomain.FriendlyName);
            owner.AddPlop(this.Plopped);
        }

        public void AddPlop(EventHandler d)
        {
            Console.WriteLine("AddPlop in domain " + AppDomain.CurrentDomain.FriendlyName);
            lock (this._events)
            {
                this._events.AddHandler(_evt_plop, d);
            }
        }

        public void RemPlop(EventHandler d)
        {
            Console.WriteLine("RemPlop in domain " + AppDomain.CurrentDomain.FriendlyName);
            lock (this._events)
            {
                this._events.RemoveHandler(_evt_plop, d);
            }
        }

        public void Raise()
        {
            Console.WriteLine("Raise in domain " + AppDomain.CurrentDomain.FriendlyName);
            EventHandler d;
            lock (this._events)
            {
                d = (EventHandler)this._events[_evt_plop];
            }
            if (d != null)
                d.Invoke(this, EventArgs.Empty);
        }

        private void Plopped(object sender, EventArgs e)
        {
            Console.WriteLine("Plopped in domain " + AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
