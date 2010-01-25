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
        /// <summary>
        /// The delegate to execute for command
        /// </summary>
        private readonly ConCommandDelegate _code;
        /// <summary>
        /// The completion delegate for command
        /// </summary>
        private readonly ConCommandCompleteDelegate _complete;

        /// <summary>
        /// The delegate to execute for command
        /// </summary>
        internal ConCommandDelegate Code { get { return this._code; } }
        /// <summary>
        /// The completion delegate for command
        /// </summary>
        internal ConCommandCompleteDelegate Complete { get { return this._complete; } }

        /// <summary>
        /// Create a managed wrapper for a ConCommand
        /// </summary>
        /// <param name="nativeId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="flags"></param>
        /// <param name="code"></param>
        /// <param name="complete"></param>
        internal ClsConCommand(UInt64 nativeId, string name, string description, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate complete)
            : base(nativeId, name, description, flags)
        {
            this._code = code;
            this._complete = complete;
        }
    }
}
