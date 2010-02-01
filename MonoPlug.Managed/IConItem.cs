using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    public interface IConItem
    {
        /// <summary>
        /// Register a convar
        /// </summary>
        /// <param name="name">Name of convar</param>
        /// <param name="help">Help text of convar</param>
        /// <param name="flags">Flags of convar</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Convar instance if success, else null.</returns>
        ClsConVar RegisterConvar(string name, string help, FCVAR flags, string defaultValue);
        /// <summary>
        /// Unregister a convar
        /// </summary>
        /// <param name="var">Convar instance</param>
        void UnregisterConvar(ClsConVar var);

        /// <summary>
        /// Register a ConCommand
        /// </summary>
        /// <param name="name">Name of command</param>
        /// <param name="help">Help text of command</param>
        /// <param name="flags">Flags of command</param>
        /// <param name="code">Code to invoke</param>
        /// <param name="completion">Auto-completion of command</param>
        /// <param name="async">True if command will be called asynchronously</param>
        /// <returns>ConCommand instance if success, else null</returns>
        ClsConCommand RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate completion, bool async);
        /// <summary>
        /// Unregister a ConCommand
        /// </summary>
        /// <param name="command">ConCommand instance to unregister</param>
        /// <returns>True if unregister is successfull</returns>
        void UnregisterConCommand(ClsConCommand cmd);
    }
}
