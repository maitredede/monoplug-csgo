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

        public virtual Task<PlayerData[]> GetPlayers()
        {
            Console.WriteLine("GetPlayers not implemented");
            return Task.FromResult<PlayerData[]>(null);
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
        private static readonly object EventLevelInit = new object();
        private static readonly object EventServerActivate = new object();
        private static readonly object EventLevelShutdown = new object();
        private static readonly object EventClientActive = new object();
        private static readonly object EventClientDisconnect = new object();
        private static readonly object EventClientPutInServer = new object();
        private static readonly object EventClientSettingsChanged = new object();
        private static readonly object EventClientConnect = new object();
        private static readonly object EventClientCommand = new object();
        private static readonly object EventGameEvent = new object();

        private void RaiseEvent<T>(object evt, T args) where T : EventArgs
        {
            Delegate d = this.m_events[evt];
            if (d == null)
                return;
            if (d is EventHandler)
                ((EventHandler)d).Invoke(this, args);
            else
                ((EventHandler<T>)d).Invoke(this, args);
        }

        event EventHandler<LevelInitEventArgs> IEngine.LevelInit
        {
            add { this.m_events.AddHandler(EventLevelInit, value); }
            remove { this.m_events.RemoveHandler(EventLevelInit, value); }
        }

        internal void RaiseLevelInit(LevelInitEventArgs e)
        {
            this.RaiseEvent(EventLevelInit, e);
        }

        event EventHandler<ServerActivateEventArgs> IEngine.ServerActivate
        {
            add { this.m_events.AddHandler(EventServerActivate, value); }
            remove { this.m_events.RemoveHandler(EventServerActivate, value); }
        }

        internal void RaiseServerActivate(ServerActivateEventArgs e)
        {
            this.RaiseEvent(EventServerActivate, e);
        }
        #endregion


        event EventHandler IEngine.LevelShutdown
        {
            add { this.m_events.AddHandler(EventLevelShutdown, value); }
            remove { this.m_events.RemoveHandler(EventLevelShutdown, value); }
        }

        internal void RaiseLevelShutdown(EventArgs e)
        {
            this.RaiseEvent(EventLevelShutdown, e);
        }

        event EventHandler IEngine.ClientActive
        {
            add { this.m_events.AddHandler(EventClientActive, value); }
            remove { this.m_events.RemoveHandler(EventClientActive, value); }
        }

        internal void RaiseClientActive(EventArgs e)
        {
            this.RaiseEvent(EventClientActive, e);
        }

        event EventHandler IEngine.ClientDisconnect
        {
            add { this.m_events.AddHandler(EventClientDisconnect, value); }
            remove { this.m_events.RemoveHandler(EventClientDisconnect, value); }
        }

        internal void RaiseClientDisconnect(EventArgs e)
        {
            this.RaiseEvent(EventClientDisconnect, e);
        }

        event EventHandler IEngine.ClientPutInServer
        {
            add { this.m_events.AddHandler(EventClientPutInServer, value); }
            remove { this.m_events.RemoveHandler(EventClientPutInServer, value); }
        }

        internal void RaiseClientPutInServer(EventArgs e)
        {
            this.RaiseEvent(EventClientPutInServer, e);
        }

        event EventHandler IEngine.ClientSettingsChanged
        {
            add { this.m_events.AddHandler(EventClientSettingsChanged, value); }
            remove { this.m_events.RemoveHandler(EventClientSettingsChanged, value); }
        }

        internal void RaiseClientSettingsChanged(EventArgs e)
        {
            this.RaiseEvent(EventClientSettingsChanged, e);
        }

        event EventHandler IEngine.ClientConnect
        {
            add { this.m_events.AddHandler(EventClientConnect, value); }
            remove { this.m_events.RemoveHandler(EventClientConnect, value); }
        }

        internal void RaiseClientConnect(EventArgs e)
        {
            this.RaiseEvent(EventClientConnect, e);
        }

        event EventHandler IEngine.ClientCommand
        {
            add { this.m_events.AddHandler(EventClientCommand, value); }
            remove { this.m_events.RemoveHandler(EventClientCommand, value); }
        }

        internal void RaiseClientCommand(EventArgs e)
        {
            this.RaiseEvent(EventClientCommand, e);
        }

        event EventHandler<GameEventEventArgs> IEngine.GameEvent
        {
            add { this.m_events.AddHandler(EventGameEvent, value); }
            remove { this.m_events.RemoveHandler(EventGameEvent, value); }
        }

        internal void RaiseGameEvent(GameEventEventArgs e)
        {
            this.RaiseEvent(EventGameEvent, e);
        }

        protected abstract void ShowMOTDInternal(int playerId, byte[] titleUTF8, byte[] msgUTF8, MOTDType type, byte[] cmdUTF8);

        Task IEngine.ShowMOTD(PlayerData player, string title, Uri url, string cmdOnClose)
        {
            int id = player.Id;
            byte[] titleUTF8 = this.m_enc.GetBytes(title);
            byte[] urlUTF8 = this.m_enc.GetBytes(url.ToString());
            byte[] cmdUTF8 = null;
            if (!string.IsNullOrEmpty(cmdOnClose))
                cmdUTF8 = this.m_enc.GetBytes(cmdOnClose);
            return this.m_fact.StartNew(() =>
            {
                this.ShowMOTDInternal(id, titleUTF8, urlUTF8, MOTDType.Url, cmdUTF8);
            });
        }
    }
}
