using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MonoPlug
{
    class ConWCFConsole
    {
        internal void Raise_ConMessage(bool hasColor, bool debug, int r, int g, int b, int a, string msg)
        {
            //Build color dic
            Dictionary<Color, ConsoleColor> _base = new Dictionary<Color, ConsoleColor>();
            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                string s = cc.ToString();
                Color c = (Color)typeof(Color).GetField(s).GetValue(null);
                _base.Add(c, cc);
            }

            float fa = 0.5f;
            float fb = 0.5f;
            float fc = 1f;

            float h1, s1, l1;

            //Find nearest color
            Color nearest = Color.FromArgb(a, r, g, b);
            h1 = nearest.GetHue();
            s1 = nearest.GetSaturation();
            l1 = nearest.GetBrightness();
            float original = fa * h1 * h1 + fb * s1 * s1 + fc * l1 * l1;
            float nearestDiff = float.MaxValue;
            foreach (Color test in _base.Keys)
            {
                h1 = test.GetHue();
                s1 = test.GetSaturation();
                l1 = test.GetBrightness();
                float testfact = fa * h1 * h1 + fb * s1 * s1 + fc * l1 * l1;
                float diff = Math.Abs(testfact - original);
                if (diff < nearestDiff)
                {
                    nearest = test;
                    nearestDiff = diff;
                }
            }

            Console.ForegroundColor = _base[nearest];
            Console.Write(msg);
        }
    }
}
