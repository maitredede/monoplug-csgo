using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal static void DumpThreadId()
        {
            string msg = string.Format("M: Thread Id is {0}\n", System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.Write(msg);
            //try
            //{
            //    Mono_Msg(string.Format("M: Thread Id is {0}\n", System.Threading.Thread.CurrentThread.ManagedThreadId));
            //}
            //catch (Exception ex)
            //{
            //    Mono_Msg("M: Thread Id Dump Error\n");
            //    Mono_Msg(ex.GetType().FullName + "\n");
            //    Mono_Msg(ex.Message + "\n");
            //    Mono_Msg(ex.StackTrace + "\n");
            //}
        }

        /// <value>
        /// Get Mono Runtime version
        /// </value>
        internal static string MonoVersion
        {
            get
            {
                Type t = Type.GetType("Mono.Runtime");
                if (t == null)
                    return "Not on Mono Runtime";
                PropertyInfo prop = t.GetProperty("Version");
                if (prop == null)
                {
                    t = Type.GetType("Consts", false, false);
                    if (t == null)
                    {
                        return ("Can't Mono.Runtime.Version or Consts.MonoVersion");
                    }
                    else
                    {
                        FieldInfo f = t.GetField("MonoVersion");
                        return (string)f.GetValue(null);
                    }
                }
                else
                {
                    object version = prop.GetValue(null, null);
                    if (version == null)
                    {
                        return "Version is null";
                    }
                    return prop.GetValue(null, null).ToString();
                }
            }
        }

        internal static string[] Explode(string args)
        {
            if (args == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(args))
            {
                return new string[] { };
            }

            char space = ' ';
            //TODO : better args split
            return args.Split(space);
        }
    }
}
