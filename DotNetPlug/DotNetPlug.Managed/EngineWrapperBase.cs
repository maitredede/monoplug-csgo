using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal abstract class EngineWrapperBase : IEngine
    {
        protected readonly PluginManager m_manager;
        protected readonly TaskFactory m_fact;
        protected readonly Encoding m_enc;

        internal EngineWrapperBase(PluginManager manager)
        {
            this.m_manager = manager;
            this.m_fact = new TaskFactory(this.m_manager.TaskScheduler);
            this.m_enc = Encoding.UTF8;

            this.m_commands = new Dictionary<int, ManagedCommand>();
        }

        private static int s_commandId = 0;

        protected readonly Dictionary<int, ManagedCommand> m_commands;

        protected ManagedCommand CreateCommand(string name, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            int id = Interlocked.Increment(ref s_commandId);
            ManagedCommand cmd = new ManagedCommand
            {
                Id = id,
                Name = name,
                Description = description,
                Flags = flags,
                Callback = callback,
            };
            lock (this.m_commands)
            {
                this.m_commands.Add(cmd.Id, cmd);
            }
            return cmd;
        }

        public virtual void RaiseCommand(int id, int argc, string[] argv)
        {
            ManagedCommand cmd;
            if (this.m_commands.TryGetValue(id, out cmd))
            {
                cmd.Callback.BeginInvoke(argv, null, null);
            }
        }

        public abstract Task<string> ExecuteCommand(string command);
        public abstract Task Log(string log);

        public virtual Task<IServerInfo> GetServerInfo()
        {
            Console.WriteLine("GetServerInfo not implemented");
            return Task.FromResult<IServerInfo>(null);
        }
        public virtual Task<IPlayer[]> GetPlayers()
        {
            Console.WriteLine("GetPlayers not implemented");
            return Task.FromResult<IPlayer[]>(null);
        }

        internal int RegisterCommandInternal(string command, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            ManagedCommand cmd = this.CreateCommand(command, description, flags, callback);

            byte[] cmdUTF8 = this.m_enc.GetBytes(command);
            byte[] descUTF8 = this.m_enc.GetBytes(description);
            int iFlags = (int)flags;

            int id = this.RegisterCommandImpl(cmdUTF8, descUTF8, iFlags, cmd.Id);
            if (id == -1)
            {
                this.UnregisterCommandDic(id);
            }
            return id;
        }

        public Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback)
        {
            ManagedCommand cmd = this.CreateCommand(command, description, flags, callback);

            byte[] cmdUTF8 = this.m_enc.GetBytes(command);
            byte[] descUTF8 = this.m_enc.GetBytes(description);
            int iFlags = (int)flags;

            return this.m_fact.StartNew(() =>
            {
                int id = this.RegisterCommandImpl(cmdUTF8, descUTF8, iFlags, cmd.Id);
                if (id == -1)
                {
                    this.UnregisterCommandDic(id);
                }
                return id;
            });
        }
        protected abstract int RegisterCommandImpl(byte[] cmdUTF8, byte[] descUTF8, int iFlags, int id);

        public Task UnregisterCommand(int id)
        {
            this.UnregisterCommandDic(id);
            return this.m_fact.StartNew(() => this.UnregisterCommandImpl(id));
        }

        protected void UnregisterCommandDic(int id)
        {
            lock (this.m_commands)
            {
                if (this.m_commands.ContainsKey(id))
                {
                    this.m_commands.Remove(id);
                }
            }
        }

        protected abstract void UnregisterCommandImpl(int id);

        internal void UnregisterCommandSync(int id)
        {
            this.UnregisterCommandDic(id);
            this.UnregisterCommandImpl(id);
        }

        #region Events
        private readonly System.ComponentModel.EventHandlerList m_events = new System.ComponentModel.EventHandlerList();
        private enum Events
        {
            LevelInit,
            ServerActivate,
            LevelShutdown,
            ClientActive,
            ClientDisconnect,
            ClientPutInServer,
            ClientSettingsChanged,
            ClientConnect,
            ClientCommand,
            GameEvent,
        }

        private void RaiseEvent<T>(Events evt, T args) where T : EventArgs
        {
            EventHandler<T> d = (EventHandler<T>)this.m_events[evt];
            if (d != null)
                d.Invoke(this, args);
        }

        public event EventHandler<LevelInitEventArgs> LevelInit
        {
            add { this.m_events.AddHandler(Events.LevelInit, value); }
            remove { this.m_events.RemoveHandler(Events.LevelInit, value); }
        }

        internal void RaiseLevelInit(LevelInitEventArgs e)
        {
            this.RaiseEvent(Events.LevelInit, e);
        }

        public event EventHandler<ServerActivateEventArgs> ServerActivate
        {
            add { this.m_events.AddHandler(Events.ServerActivate, value); }
            remove { this.m_events.RemoveHandler(Events.ServerActivate, value); }
        }

        internal void RaiseServerActivate(ServerActivateEventArgs e)
        {
            this.RaiseEvent(Events.ServerActivate, e);
        }
        #endregion


        public event EventHandler LevelShutdown
        {
            add { this.m_events.AddHandler(Events.LevelShutdown, value); }
            remove { this.m_events.RemoveHandler(Events.LevelShutdown, value); }
        }

        internal void RaiseLevelShutdown(EventArgs e)
        {
            this.RaiseEvent(Events.LevelShutdown, e);
        }

        public event EventHandler ClientActive
        {
            add { this.m_events.AddHandler(Events.ClientActive, value); }
            remove { this.m_events.RemoveHandler(Events.ClientActive, value); }
        }

        internal void RaiseClientActive(EventArgs e)
        {
            this.RaiseEvent(Events.ClientActive, e);
        }

        public event EventHandler ClientDisconnect
        {
            add { this.m_events.AddHandler(Events.ClientDisconnect, value); }
            remove { this.m_events.RemoveHandler(Events.ClientDisconnect, value); }
        }

        internal void RaiseClientDisconnect(EventArgs e)
        {
            this.RaiseEvent(Events.ClientDisconnect, e);
        }

        public event EventHandler ClientPutInServer
        {
            add { this.m_events.AddHandler(Events.ClientPutInServer, value); }
            remove { this.m_events.RemoveHandler(Events.ClientPutInServer, value); }
        }

        internal void RaiseClientPutInServer(EventArgs e)
        {
            this.RaiseEvent(Events.ClientPutInServer, e);
        }

        public event EventHandler ClientSettingsChanged
        {
            add { this.m_events.AddHandler(Events.ClientSettingsChanged, value); }
            remove { this.m_events.RemoveHandler(Events.ClientSettingsChanged, value); }
        }

        internal void RaiseClientSettingsChanged(EventArgs e)
        {
            this.RaiseEvent(Events.ClientSettingsChanged, e);
        }

        public event EventHandler ClientConnect
        {
            add { this.m_events.AddHandler(Events.ClientConnect, value); }
            remove { this.m_events.RemoveHandler(Events.ClientConnect, value); }
        }

        internal void RaiseClientConnect(EventArgs e)
        {
            this.RaiseEvent(Events.ClientConnect, e);
        }

        public event EventHandler ClientCommand
        {
            add { this.m_events.AddHandler(Events.ClientCommand, value); }
            remove { this.m_events.RemoveHandler(Events.ClientCommand, value); }
        }

        internal void RaiseClientCommand(EventArgs e)
        {
            this.RaiseEvent(Events.ClientCommand, e);
        }

        public event EventHandler<GameEventEventArgs> GameEvent
        {
            add { this.m_events.AddHandler(Events.GameEvent, value); }
            remove { this.m_events.RemoveHandler(Events.GameEvent, value); }
        }

        internal void RaiseGameEvent(GameEventEventArgs e)
        {
            this.RaiseEvent(Events.GameEvent, e);
        }
    }
}
