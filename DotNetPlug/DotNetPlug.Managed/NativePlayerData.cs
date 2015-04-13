using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [DebuggerDisplay("#{id} {name}")]
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class NativePlayerData
    {
        public int id;
        [MarshalAs(UnmanagedType.BStr)]
        public string name;
        public int team;
        public int health;
        [MarshalAs(UnmanagedType.BStr)]
        public string ip_address;
        [MarshalAs(UnmanagedType.BStr)]
        public string steam_id;

        [MarshalAs(UnmanagedType.I1)]
        public bool is_bot;
        [MarshalAs(UnmanagedType.I1)]
        public bool is_dead;
    }
}
