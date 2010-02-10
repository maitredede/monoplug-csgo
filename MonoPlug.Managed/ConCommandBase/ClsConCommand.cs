﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// A ConCommand instance
    /// </summary>
    public sealed class ClsConCommand : ClsConCommandBase
    {
        /// <summary>
        /// The delegate to execute for command
        /// </summary>
        internal ConCommandDelegate Code { get { return this._icmd.Code; } }
        /// <summary>
        /// The completion delegate for command
        /// </summary>
        internal ConCommandCompleteDelegate Complete { get { return this._icmd.Complete; } }
        /// <summary>
        /// Command should be executed asynchronously
        /// </summary>
        internal bool Async { get { return this._icmd.Async; } }

        private readonly InternalConCommand _icmd;

        internal InternalConCommand Internal { get { return this._icmd; } }

        /// <summary>
        /// Create a managed wrapper for a ConCommand
        /// </summary>
        internal ClsConCommand(InternalConCommand icmd, ClsPluginBase owner)
            : base(icmd, owner)
        {
#if DEBUG
            Check.NonNull("icmd", icmd);
#endif
            this._icmd = icmd;
        }

        internal void Execute(ClsPlayer sender, string line, string[] args)
        {
#if DEBUG
            NativeMethods.Mono_DevMsg(string.Format("ClsConCommand::Execute async={0}\n", this.Async));
#endif
            if (this.Async)
            {
                this.Plugin.ThreadPool.QueueUserWorkItem<ClsPlayer, string, string[]>(this.Exec, sender, line, args);
            }
            else
            {
                this.Exec(sender, line, args);
            }
        }

        private void Exec(ClsPlayer sender, string line, string[] args)
        {
#if DEBUG
            NativeMethods.Mono_DevMsg(string.Format("ClsConCommand::Exec async={0}\n", this.Async));
#endif
            this.Code(sender, line, args);
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("ConCommand: {0} {1}", this.Name, this.Async ? "Async" : "Sync");
        }
#endif
    }
}
