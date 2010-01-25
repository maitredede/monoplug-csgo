using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    internal sealed class ClsThreadItem<TRet, TParam> : MarshalByRefObject, IExecute
    {
        private readonly ManualResetEvent _latch = new ManualResetEvent(false);
        private readonly InterThreadCallDelegate<TRet, TParam> _code;
        private readonly TParam _param;
        private TRet _return;

        internal ClsThreadItem(InterThreadCallDelegate<TRet, TParam> code, TParam param)
        {
            this._code = code;
            this._param = param;
        }

        public void WaitOne()
        {
            this._latch.WaitOne();
        }

        public TRet ReturnValue { get { return this._return; } }

        public void Execute()
        {
            try
            {
#if DEBUG
                NativeMethods.Mono_DevMsg(string.Format("ClsThreadItem::Execute() in [{0}]\n", AppDomain.CurrentDomain.FriendlyName));
#endif
                this._return = this._code(this._param);
            }
            finally
            {
                this._latch.Set();
            }
        }
    }
}
