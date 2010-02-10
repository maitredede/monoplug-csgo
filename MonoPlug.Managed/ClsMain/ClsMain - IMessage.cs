using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsMain : IMessage
    {
        void IMessage.DevMsg(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            this.InterThreadCall<object, string>(this.DevMsg_CALL, format);
        }

        private object DevMsg_CALL(string msg)
        {
            NativeMethods.Mono_Msg(msg);
            return null;
        }

        void IMessage.Error(Exception ex)
        {
            ((IMessage)this).Error(this.GetExceptionText(ex));
        }

        internal string GetExceptionText(Exception ex)
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

        void IMessage.Error(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            this.InterThreadCall<object, string>(this.Error_CALL, format);
        }

        private object Error_CALL(string msg)
        {
            NativeMethods.Mono_Error(msg);
            return null;
        }

        void IMessage.Warning(Exception ex)
        {
            ((IMessage)this).Warning(this.GetExceptionText(ex));
        }

        void IMessage.Warning(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            this.InterThreadCall<object, string>(this.Warning_CALL, format);
        }

        private object Warning_CALL(string msg)
        {
            NativeMethods.Mono_Warning(msg);
            return null;
        }

        void IMessage.Msg(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            this.InterThreadCall<object, string>(this.MsgCall, format);
        }

        private object MsgCall(string msg)
        {
            NativeMethods.Mono_Msg(msg);
            return null;
        }

        void IMessage.Log(string format, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                format = string.Format(format, args);
            }
            this.InterThreadCall<object, string>(this.LogCall, format);
        }

        private object LogCall(string msg)
        {
            NativeMethods.Mono_Log(msg);
            return null;
        }
    }
}
