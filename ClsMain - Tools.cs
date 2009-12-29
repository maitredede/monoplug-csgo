using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
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

        /// <summary>
        /// Validate FCVAR flags value
        /// </summary>
        /// <param name="flags">flags to validate</param>
        /// <param name="argName">name of argument</param>
        internal static void ValidateFlags(FCVAR flags, string argName)
        {
            FCVAR all = FCVAR.FCVAR_NONE;
            foreach (FCVAR value in Enum.GetValues(typeof(FCVAR)))
            {
                all |= value;
            }
            int revall = ((int)all) ^ -1;
            int calc = ((int)flags) & revall;
            if (calc != 0)
            {
                throw new ArgumentOutOfRangeException(argName);
            }
        }

        internal static bool IsMethodOfSignature(MethodInfo method, Type delegateSignature, object instance)
        {
            if (method == null) throw new ArgumentNullException("method");
            if (delegateSignature == null) throw new ArgumentNullException("delegateSignature");
            if (delegateSignature.BaseType != typeof(MulticastDelegate)) throw new ArgumentException("Not a delegate", "delegateSignature");

            try
            {
                Delegate.CreateDelegate(delegateSignature, instance, method);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        internal static bool IsMethodOfSignature(MethodInfo method, Type delegateSignature)
        {
            return IsMethodOfSignature(method, delegateSignature, null);
        }
    }
}
