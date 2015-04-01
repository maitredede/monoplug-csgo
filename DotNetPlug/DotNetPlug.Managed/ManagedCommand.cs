using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [DebuggerDisplay("Cmd #{Id} : {Name}")]
    internal sealed class ManagedCommand
    {
        /// <summary>
        /// Managed id
        /// </summary>
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CommandExecuteDelegate Callback { get; set; }
        public FCVar Flags { get; set; }
    }
}
