
using System;

namespace MonoPlug
{
    /// <summary>
    /// Delegate for managed ConCommands
    /// </summary>
    /// <param name="args">Arguments of command</param>
    public delegate void ConCommandDelegate(string args);

    /// <summary>
    /// Delegate for managed ConCommands auto-completion
    /// </summary>
    /// <param name="partial">Current input of user (including command)</param>
    /// <returns>Array of completion results</returns>
    public delegate string[] ConCommandCompleteDelegate(string partial);
}
