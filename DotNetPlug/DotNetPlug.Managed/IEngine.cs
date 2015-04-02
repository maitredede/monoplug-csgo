using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Interface for engine interaction
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Execute a server command as if it was typed on server console
        /// </summary>
        /// <param name="command">Server command</param>
        /// <returns>Command result</returns>
        Task<string> ExecuteCommand(string command);

        /// <summary>
        /// Append to game log
        /// </summary>
        /// <param name="engine">Game engine</param>
        Task Log(string log);

        /// <summary>
        /// Registers a managed command.
        /// </summary>
        /// <param name="command">The command name</param>
        /// <param name="description">The command description.</param>
        /// <param name="flags">The command flags.</param>
        /// <param name="callback">Method called for command execution</param>
        /// <returns>Command Id for unregistration</returns>
        Task<int> RegisterCommand(string command, string description, FCVar flags, CommandExecuteDelegate callback);

        /// <summary>
        /// Unregisters a command.
        /// </summary>
        /// <param name="id">The command identifier.</param>
        /// <returns></returns>
        Task UnregisterCommand(int id);

        /// <summary>
        /// Gets the players.
        /// </summary>
        /// <returns></returns>
        Task<IPlayer[]> GetPlayers();

        /// <summary>
        /// Gets the server information.
        /// </summary>
        /// <returns></returns>
        Task<IServerInfo> GetServerInfo();

        event EventHandler<LevelInitEventArgs> LevelInit;
        event EventHandler<ServerActivateEventArgs> ServerActivate;
    }
}
