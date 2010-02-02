using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    /// <summary>
    /// Interface for console output
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Msg
        /// </summary>
        /// <param name="format"></param>
        /// <param name="elements"></param>
        void Msg(string format, params object[] elements);
        /// <summary>
        /// DevMsg
        /// </summary>
        /// <param name="format"></param>
        /// <param name="elements"></param>
        void DevMsg(string format, params object[] elements);
        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="format"></param>
        /// <param name="elements"></param>
        void Warning(string format, params object[] elements);
        /// <summary>
        /// Warning
        /// </summary>
        /// <param name="ex"></param>
        void Warning(Exception ex);
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="format"></param>
        /// <param name="elements"></param>
        void Error(string format, params object[] elements);
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="ex"></param>
        void Error(Exception ex);
        /// <summary>
        /// Log message
        /// </summary>
        /// <param name="format"></param>
        /// <param name="elements"></param>
        void Log(string format, params object[] elements);
    }
}
