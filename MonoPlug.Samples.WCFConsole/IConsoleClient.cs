using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ServiceModel;

namespace MonoPlug
{
    /// <summary>
    /// WCFConsole client callback interface
    /// </summary>
    [ServiceContract]
    public interface IConsoleClient
    {
        /// <summary>
        /// Called when starting message reception
        /// </summary>
        /// <param name="hasColor">Message has color</param>
        /// <param name="debug">Debug message</param>
        /// <param name="color">Message color</param>
        /// <param name="message">Message text</param>
        /// <param name="cb">Callback</param>
        /// <param name="s">State</param>
        /// <returns></returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginConsoleMessage(bool hasColor, bool debug, Color color, string message, AsyncCallback cb, object s);
        /// <summary>
        /// End message reception
        /// </summary>
        /// <param name="result"></param>
        void EndConsoleMessage(IAsyncResult result);
    }
}
