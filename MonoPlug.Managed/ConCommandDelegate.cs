
using System;

namespace MonoPlug
{
    /// <summary>
    /// Delegate for managed ConCommands
    /// </summary>
    /// <param name="sender">Player that has sent the command (null = console)</param>
    /// <param name="line">Full command line</param>
    /// <param name="arguments">Tockenized command line</param>
    public delegate void ConCommandDelegate(ClsPlayer sender, string line, string[] arguments);

    /// <summary>
    /// Delegate for managed ConCommands auto-completion
    /// </summary>
    /// <param name="partial">Current input of user (including command)</param>
    /// <returns>Array of completion results</returns>
    public delegate string[] ConCommandCompleteDelegate(string partial);
}
