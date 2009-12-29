using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Test
{
    delegate void d(string str);

    abstract class ABase
    {
    }

    class BBase : ABase
    {
    }

    class Program
    {
        static void Main(string[] args)
        {
            ABase a = new BBase();
            Console.WriteLine(a.GetType().FullName);
        }
    }
}
