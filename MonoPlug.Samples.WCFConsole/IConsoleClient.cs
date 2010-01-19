using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ServiceModel;

namespace MonoPlug
{
    [ServiceContract]
    public interface IConsoleClient
    {
        [OperationContract(IsOneWay = true)]
        void ConsoleMessage(bool hasColor, bool debug, Color color, string message);
    }
}
