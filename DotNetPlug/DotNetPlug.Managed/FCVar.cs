using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Game commands or CVar flags
    /// </summary>
    [Flags]
    public enum FCVar
    {
        /// <summary>
        /// The default, no flags at all
        /// </summary>
        None = 0,
        /// <summary>
        /// If this is set, don't add to linked list, etc
        /// </summary>
        Unregistered = 1 << 0,
        /// <summary>
        /// Hidden in released products. Flag is removed automatically if ALLOW_DEVELOPMENT_CVARS is defined.
        /// </summary>
        DevelopmentOnly = 1 << 1,
        GameDll = 1 << 2,
        ClientDll = 1 << 3,
        Hidden = 1 << 4,

        Protected = 1 << 5,
        SPOnly = 1 << 6,
        Archive = 1 << 7,
        Notify = 1 << 8,
        UserInfo = 1 << 9,
        PrintableOnly = 1 << 10,
        Unlogged = 1 << 11,
        NeverAsString = 1 << 12,

        Replicated = 1 << 13,
        Cheat = 1 << 14,
        SS = 1 << 15,
        Demo = 1 << 16,
        DontRecord = 1 << 17,
        SS_Added = 1 << 18,
        Release = 1 << 19,
        ReloadMaterials = 1 << 20,
        ReloadTextures = 1 << 21,

        NotConnected = 1 << 22,
        MaterialSystemThread = 1 << 23,
        ArchiveXBox = 1 << 24,

        ServerCanExecute = 1 << 28,
        ServerCannotQuery = 1 << 29,
        ClientCmdCanExecute = 1 << 30,

        AccessibleFromThreads = 1 << 25,

        //Available = 1 << 26,
        //Available = 1 << 27,
        //Available = 1 << 31,

        MaterialThreadMask = ReloadMaterials | ReloadTextures | MaterialSystemThread,
    }
}
