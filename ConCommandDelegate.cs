
using System;

namespace MonoPlug
{
    public delegate void ConCommandDelegate(string args);
    public delegate string[] ConCommandCompleteDelegate(string partial);
}
