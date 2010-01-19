using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MonoPlug
{
    [ServiceContract(CallbackContract = typeof(IConsoleClient), Name = "WCFConsole")]
    public interface IConsoleServer
    {
        [OperationContract(IsOneWay = true)]
        void Ping();
    }
}
