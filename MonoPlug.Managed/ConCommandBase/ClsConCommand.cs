using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// A ConCommand instance, created using the <see cref="MonoPlug.ClsPluginBase.RegisterConCommand(System.String,System.String,MonoPlug.FCVAR,MonoPlug.ConCommandDelegate,MonoPlug.ConCommandCompleteDelegate)"/> function.
    /// </summary>
    public sealed class ClsConCommand : ClsConCommandBase
    {
        private readonly ConCommandDelegate _code;
        private readonly ConCommandCompleteDelegate _complete;
        private readonly bool _async;

        /// <summary>
        /// The delegate to execute for command
        /// </summary>
        internal ConCommandDelegate Code { get { return this._code; } }
        /// <summary>
        /// The completion delegate for command
        /// </summary>
        internal ConCommandCompleteDelegate Complete { get { return this._complete; } }

        internal bool Async { get { return this._async; } }

        /// <summary>
        /// Create a managed wrapper for a ConCommand
        /// </summary>
        internal ClsConCommand(IMessage msg, IThreadPool pool, ClsPluginBase plugin, string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete, bool async)
            : base(msg, pool, plugin, name, help, flags)
        {
            this._code = code;
            this._complete = complete;
            this._async = async;
        }

        internal void Execute(string line, string[] args)
        {
#if DEBUG
            this.Msg.DevMsg("Execute {0}\n", this.Name);
#endif
            this.Execute(line, args, this._async);
        }

        internal void Execute(string line, string[] args, bool async)
        {
            if (async)
            {
#if DEBUG
                this.Msg.DevMsg("Queuing async {0}\n", this.Name);
#endif
                this.ThreadPool.QueueUserWorkItem<string, string[]>(this.ExecuteSync, line, args);
            }
            else
            {
                this.ExecuteSync(line, args);
            }
        }

        private void ExecuteSync(string line, string[] args)
        {
#if DEBUG
            this.Msg.DevMsg("Executing {0}\n", this.Name);
#endif
            try
            {
                this._code(line, args);
            }
            catch (Exception ex)
            {
                this.Msg.Warning(ex);
            }
        }
    }
}
