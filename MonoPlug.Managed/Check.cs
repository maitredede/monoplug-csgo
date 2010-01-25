using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MonoPlug
{
    internal static class Check
    {
        /// <summary>
        /// Validate FCVAR flags value
        /// </summary>
        /// <param name="flags">flags to validate</param>
        /// <param name="argName">name of argument</param>
        internal static void ValidFlags(FCVAR flags, string argName)
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
            NonNull("method", method);
            NonNull("delegateSignature", delegateSignature);
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

        internal static void NonNull<T>(string name, T value) where T : class
        {
            if (value == null) throw new ArgumentNullException(name);
        }

        internal static void NonNullOrEmpty(string name, string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(name);
        }

        internal static void InRange(string name, int value, int min, int max)
        {
            if (value < min || value > max) throw new ArgumentOutOfRangeException(name);
        }

        internal static void InRange(string name, ulong value, ulong min, ulong max)
        {
            if (value < min || value > max) throw new ArgumentOutOfRangeException(name);
        }
    }
}
