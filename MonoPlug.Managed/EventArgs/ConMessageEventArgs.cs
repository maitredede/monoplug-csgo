using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    /// <summary>
    /// Console Message
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    [System.Diagnostics.DebuggerDisplay("({HasColor},{Color.R},{Color.G},{Color.B}) dbg={IsDebug} : {Message}")]
    public sealed class ConMessageEventArgs : EventArgs
    {
        private bool _hasColor;
        private bool _debug;
        private Color _color;
        private string _msg;

        /// <summary>
        /// True if message has color data
        /// </summary>
        public bool HasColor { get { return this._hasColor; } }
        /// <summary>
        /// True if it is a debug message
        /// </summary>
        public bool IsDebug { get { return this._debug; } }
        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get { return this._msg; } }
        /// <summary>
        /// Message color
        /// </summary>
        public Color Color { get { return this._color; } }

        public ConMessageEventArgs(bool hasColor, bool isDebug, Color color, string msg)
        {
            this._hasColor = hasColor;
            this._debug = isDebug;
            this._color = color;
            this._msg = msg;
        }
    }
}
