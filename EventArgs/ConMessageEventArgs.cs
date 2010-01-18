using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    [System.Diagnostics.DebuggerStepThrough]
    [System.Diagnostics.DebuggerDisplay("({HasColor},{Color.R},{Color.G},{Color.B}) dbg={IsDebug} : {Message}")]
    public sealed class ConMessageEventArgs : EventArgs
    {
        private bool _hasColor;
        private bool _debug;
        private Color _color;
        private string _msg;

        public bool HasColor { get { return this._hasColor; } }
        public bool IsDebug { get { return this._debug; } }
        public string Message { get { return this._msg; } }
        public Color Color { get { return this._color; } }

        internal ConMessageEventArgs(bool hasColor, bool isDebug, Color color, string msg)
        {
            this._hasColor = hasColor;
            this._debug = isDebug;
            this._color = color;
            this._msg = msg;
        }
    }
}
