using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public interface IConsolePlugin
    {
        void ColorPrint(Color color, string message);
        void Print(string message);
        void DPrint(string message);
        string GetConsoleText();
    }
}
