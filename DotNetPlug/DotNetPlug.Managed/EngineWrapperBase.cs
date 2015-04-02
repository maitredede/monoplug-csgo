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

        public Task UnregisterCommand(int id)
        {
            lock (this.m_commands)
            {
                if (this.m_commands.ContainsKey(id))
                {
                    this.m_commands.Remove(id);
                }
            }
            return Task.FromResult(id);
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
        public abstract Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback);

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

        private static object evtLevelInit = new object();
        private readonly System.ComponentModel.EventHandlerList m_events = new System.ComponentModel.EventHandlerList();

        public event EventHandler<LevelInitEventArgs> LevelInit
        {
            add { this.m_events.AddHandler(evtLevelInit, value); }
            remove { this.m_events.RemoveHandler(evtLevelInit, value); }
        }

        internal void RaiseLevelInit(LevelInitEventArgs e)
        {
            EventHandler<LevelInitEventArgs> d = (EventHandler<LevelInitEventArgs>)this.m_events[evtLevelInit];
            if (d != null)
                d.Invoke(this, e);
        }

    }
}
