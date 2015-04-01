using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void ExecuteCommandDelegate(
        [In]byte[] msgUTF8,
        [Out]out IntPtr output,
        [Out]out int length);
}
