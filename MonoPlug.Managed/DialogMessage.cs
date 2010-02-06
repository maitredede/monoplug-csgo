using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    internal struct DialogMessage
    {
        private ClsPlayer _player;
        private string _title;
        private string _message;
        private Color _color;
        private int _level;
        private int _time;

        public ClsPlayer Client { get { return this._player; } set { this._player = value; } }
        public string Title { get { return this._title; } set { this._title = value; } }
        public string Message { get { return this._message; } set { this._message = value; } }
        public Color Color { get { return this._color; } set { this._color = value; } }
        public int Level { get { return this._level; } set { this._level = value; } }
        public int Time { get { return this._time; } set { this._time = value; } }
    }
}
