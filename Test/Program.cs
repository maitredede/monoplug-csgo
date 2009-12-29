using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Test
{
    delegate void d(string str);

    class Program
    {
        static void Main(string[] args)
        {
            Type t = typeof(d);
            foreach (MethodInfo m in typeof(Program).GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                try
                {
                    Delegate.CreateDelegate(t, m);
                }
                catch (ArgumentException)
                {
                }
            }
        }

        public static void tata(string str)
        {
        }

        public static string toto(string str)
        {
            return str;
        }
    }
}
