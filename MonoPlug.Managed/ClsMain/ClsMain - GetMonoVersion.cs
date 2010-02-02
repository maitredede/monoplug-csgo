using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        internal static string GetMonoVersion()
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
}
