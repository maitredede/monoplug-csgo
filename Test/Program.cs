using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain dom = null;
            try
            {
                ClsMain main = new ClsMain();

                dom = AppDomain.CreateDomain("RemotePlop");

                dom.Load(Assembly.GetExecutingAssembly().FullName);
                ClsMain remote = (ClsMain)dom.CreateInstanceAndUnwrap(typeof(ClsMain).Assembly.FullName, typeof(ClsMain).FullName);
                remote.Start(main);
                main.Raise();
                Console.WriteLine("Raised !");
            }
            finally
            {
                AppDomain.Unload(dom);
            }
            Console.ReadLine();
        }
    }
}
