using System;
using System.Collections.Generic;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// ConVar flags
    /// </summary>
    [Flags]
    public enum FCVAR : int
    {
        /// <summary>
        /// The default, no flags at all
        /// </summary>
        FCVAR_NONE = 0,
        /// <summary>
        /// If this is set, don't add to linked list, etc.
        /// </summary>
        FCVAR_UNREGISTERED = 1 << 0,
        /// <summary>
        /// Hidden in released products. Flag is removed automatically if ALLOW_DEVELOPMENT_CVARS is defined.
        /// </summary>
        FCVAR_DEVELOPMENTONLY = 1 << 1,	// Hidden in released products. Flag is removed automatically if ALLOW_DEVELOPMENT_CVARS is defined.
        /// <summary>
        /// defined by the game DLL
        /// </summary>
        FCVAR_GAMEDLL = 1 << 2,	// defined by the game DLL
        /// <summary>
        /// defined by the client DLL
        /// </summary>
        FCVAR_CLIENTDLL = 1 << 3,  // defined by the client DLL
        /// <summary>
        /// Hidden. Doesn't appear in find or autocomplete. Like DEVELOPMENTONLY, but can't be compiled out.
        /// </summary>
        FCVAR_HIDDEN = 1 << 4,	// Hidden. Doesn't appear in find or autocomplete. Like DEVELOPMENTONLY, but can't be compiled out.

        // ConVar only
        FCVAR_PROTECTED = 1 << 5,  // It's a server cvar, but we don't send the data since it's a password, etc.  Sends 1 if it's not bland/zero, 0 otherwise as value
        FCVAR_SPONLY = 1 << 6,  // This cvar cannot be changed by clients connected to a multiplayer server.
        FCVAR_ARCHIVE = 1 << 7,	// set to cause it to be saved to vars.rc
        FCVAR_NOTIFY = 1 << 8,	// notifies players when changed
        FCVAR_USERINFO = 1 << 9,	// changes the client's info string
        FCVAR_CHEAT = 1 << 14, // Only useable in singleplayer / debug / multiplayer & sv_cheats
        FCVAR_PRINTABLEONLY = 1 << 10, // This cvar's string cannot contain unprintable characters ( e.g., used for player name etc ).
        FCVAR_UNLOGGED = 1 << 11, // If this is a FCVAR_SERVER, don't log changes to the log file / console if we are creating a log
        FCVAR_NEVER_AS_STRING = 1 << 12, // never try to print that cvar


        FCVAR_REPLICATED = 1 << 13,	// server setting enforced on clients, TODO rename to FCAR_SERVER at some time
        FCVAR_DEMO = 1 << 16,  // record this cvar when starting a demo file
        FCVAR_DONTRECORD = 1 << 17,  // don't record these command in demofiles
        FCVAR_NOT_CONNECTED = 1 << 22,	// cvar cannot be changed by a client that is connected to a server
        FCVAR_ARCHIVE_XBOX = 1 << 24, // cvar written to config.cfg on the Xbox
        /// <summary>
        /// the server is allowed to execute this command on clients via ClientCommand/NET_StringCmd/CBaseClientState::ProcessStringCmd.
        /// </summary>
        FCVAR_SERVER_CAN_EXECUTE = 1 << 28,// the server is allowed to execute this command on clients via ClientCommand/NET_StringCmd/CBaseClientState::ProcessStringCmd.
        FCVAR_SERVER_CANNOT_QUERY = 1 << 29,// If this is set, then the server is not allowed to query this cvar's value (via IServerPluginHelpers::StartQueryCvarValue).
        FCVAR_CLIENTCMD_CAN_EXECUTE = 1 << 30,	// IVEngineClient::ClientCmd is allowed to execute this command. 
    }
}