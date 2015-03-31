using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    //internal delegate void RegisterCommandDelegate(byte[] command, byte[] description, int flags, IntPtr callback);
    internal delegate int RegisterCommandDelegate(byte[] command, byte[] description, int flags, IntPtr ptr);
}
