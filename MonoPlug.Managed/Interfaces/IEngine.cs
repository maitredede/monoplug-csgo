using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Interface for engine functions
    /// </summary>
    public interface IEngine
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
        void UnregisterConCommand(ClsConCommand command);
        /// <summary>
        /// Get active players
        /// </summary>
        /// <returns></returns>
        ClsPlayer[] GetPlayers();
        /// <summary>
        /// Issue a server command
        /// </summary>
        /// <param name="command">Command to send</param>
        void ServerCommand(string command);
        /// <summary>
        /// Print a console message on client
        /// </summary>
        /// <param name="client">Target client</param>
        /// <param name="format">Message format</param>
        /// <param name="args">Message arguments</param>
        void ClientMessage(ClsPlayer client, string format, params object[] args);
        /// <summary>
        /// Register a say command
        /// </summary>
        /// <param name="trigger">Trigger text</param>
        /// <param name="async">Command is executed asynchronously</param>
        /// <param name="hidden">Prevent text from being shown</param>
        /// <returns>SayCommand instance, or NULL if failed</returns>
        ClsSayCommand RegisterSayCommand(string trigger, bool async, bool hidden);
        /// <summary>
        /// Unregister a say command
        /// </summary>
        /// <param name="command">Command to unregister</param>
        void UnregisterSayCommand(ClsSayCommand command);
    }
}
