using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal interface IConVarValue : IMessage
    {
        string GetValueString(UInt64 nativeID);
        void SetValueString(UInt64 nativeID, string value);
        bool GetValueBool(UInt64 nativeID);
        void SetValueBool(UInt64 nativeID, bool value);
    }
}
