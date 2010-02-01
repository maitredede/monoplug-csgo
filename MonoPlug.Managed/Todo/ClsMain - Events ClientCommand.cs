using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;

namespace MonoPlug
{
    partial class ClsMain
    {
        //private static readonly object _evtClientCommand = new object();

        //internal void ClientCommand_Add(ClsPluginBase plugin)
        //{
        //    this.Event_Add(plugin, _evtClientCommand, NativeMethods.Attach_ClientCommand);
        //}

        //internal void ClientCommand_Remove(ClsPluginBase plugin)
        //{
        //    this.Event_Remove(plugin, _evtClientCommand, NativeMethods.Detach_ClientCommand);
        //}

        //private void Raise_ClientCommand(ClsPlayer player, string command)
        //{
        //    Msg("M: Raise_ClientCommand {0} {1}\n", player, command);
        //    List<ClsPluginBase> lst = null;
        //    lock (this._events)
        //    {
        //        if (this._events.ContainsKey(_evtClientCommand))
        //        {
        //            lst = new List<ClsPluginBase>(this._events[_evtClientCommand]);
        //        }
        //    }
        //    if (lst != null && lst.Count > 0)
        //    {
        //        ClientCommandEventArgs e = new ClientCommandEventArgs(player, command);
        //        foreach (ClsPluginBase plugin in lst)
        //        {
        //            plugin.Raise_ClientCommand(this, e);
        //        }
        //    }
        //}
    }
}
