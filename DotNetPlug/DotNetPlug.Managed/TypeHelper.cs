using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    internal static class TypeHelper
    {
        public static ExpandoObject ExpandoNew()
        {
            return new ExpandoObject();
        }

        public static void ExpandoAdd(ExpandoObject obj, string name, object value)
        {
            ((IDictionary<string, object>)obj).Add(name, value);
        }
    }
}
