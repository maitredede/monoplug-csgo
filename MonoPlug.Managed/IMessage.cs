using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    public interface IMessage
    {
        void Msg(string format, params object[] elements);
        void DevMsg(string format, params object[] elements);
        void Warning(string format, params object[] elements);
        void Error(string format, params object[] elements);
        void Error(Exception ex);
    }
}
