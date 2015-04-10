using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [DebuggerDisplay("#{Id} {Name}")]
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class NativePlayerData : IPlayer
    {
        public int Id { get; set; }
        [MarshalAs(UnmanagedType.BStr)]
        public string Name { get; set; }
    }
}
