﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    [UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool RegisterCommandDelegate(byte[] commandUTF8, byte[] descriptionUTF8, int flags, int managedId);
}