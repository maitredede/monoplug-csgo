using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    internal sealed class ClsMessage : MarshalByRefObject, IMessage
    {
        private readonly ClsMain _main;

        internal ClsMessage(ClsMain main)
        {
            this._main = main;
        }

        public void DevMsg(string format, params object[] elements)
        {
            string msg;
            if (elements != null && elements.Length > 0)
            {
                msg = string.Format(format, elements);
            }
            else
            {
                msg = format;
            }
            this._main.InterThreadCall<object, string>(this.DevMsg_CALL, msg);
        }

        private object DevMsg_CALL(string msg)
        {
            NativeMethods.Mono_Msg(msg);
            return null;
        }

        public void Error(Exception ex)
        {
            this.Error(this.GetExceptionText(ex));
        }

        private string GetExceptionText(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendFormat("-- {0} --\n", ex.GetType().FullName);
                sb.AppendFormat("{0}\n", ex.Message);
                sb.AppendFormat("{0}\n", ex.StackTrace);
                ex = ex.InnerException;
                if (ex == null)
                {
                    sb.AppendFormat("---------------\n");
                }
            }
            return sb.ToString();
        }

        public void Error(string format, params object[] elements)
        {
            string msg;
            if (elements != null && elements.Length > 0)
            {
                msg = string.Format(format, elements);
            }
            else
            {
                msg = format;
            }
            this._main.InterThreadCall<object, string>(this.Error_CALL, msg);
        }

        private object Error_CALL(string msg)
        {
            NativeMethods.Mono_Error(msg);
            return null;
        }

        public void Warning(Exception ex)
        {
            this.Warning(this.GetExceptionText(ex));
        }

        public void Warning(string format, params object[] elements)
        {
            string msg;
            if (elements != null && elements.Length > 0)
            {
                msg = string.Format(format, elements);
            }
            else
            {
                msg = format;
            }
            this._main.InterThreadCall<object, string>(this.Warning_CALL, msg);
        }

        private object Warning_CALL(string msg)
        {
            NativeMethods.Mono_Warning(msg);
            return null;
        }

        public void Msg(string format, params object[] vals)
        {
            string msg;
            if (vals == null || vals.Length == 0)
            {
                msg = format;
            }
            else
            {
                msg = string.Format(format, vals);
            }
            this._main.InterThreadCall<object, string>(this.MsgCall, msg);
        }

        private object MsgCall(string msg)
        {
            NativeMethods.Mono_Msg(msg);
            return null;
        }

        public void Log(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            this._main.InterThreadCall<object, string>(this.LogCall, format);
        }

        private object LogCall(string msg)
        {
            NativeMethods.Mono_Log(msg);
            return null;
        }
    }
}
