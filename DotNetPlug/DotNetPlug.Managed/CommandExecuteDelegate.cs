using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    /// <summary>
    /// Delegate called when a managed command is called
    /// </summary>
    /// <param name="args">The arguments. Index 0 is command name</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CommandExecuteDelegate(string[] args);
}
