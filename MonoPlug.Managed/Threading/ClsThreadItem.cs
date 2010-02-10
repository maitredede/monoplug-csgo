﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MonoPlug
{
    internal sealed class ClsThreadItem<TRet, TParam> : ObjectBase, IExecute
    {
        private readonly ManualResetEvent _latch = new ManualResetEvent(false);
        private readonly InterThreadCallDelegate<TRet, TParam> _code;
        private readonly TParam _param;
        private TRet _return;
        private readonly IMessage _msg;

        internal ClsThreadItem(IMessage msg, InterThreadCallDelegate<TRet, TParam> code, TParam param)
        {
            this._code = code;
            this._param = param;
            this._msg = msg;
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
                this._return = this._code(this._param);
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
            }
            finally
            {
                this._latch.Set();
            }
        }
    }
}
