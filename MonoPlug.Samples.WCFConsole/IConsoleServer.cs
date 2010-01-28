using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MonoPlug
{
    /// <summary>
    /// WCFConsole server interface
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IConsoleClient), Name = "WCFConsole")]
    public interface IConsoleServer
    {
        /// <summary>
        /// Ping the server
        /// </summary>
        [OperationContract]
        void Ping();
    }
}
