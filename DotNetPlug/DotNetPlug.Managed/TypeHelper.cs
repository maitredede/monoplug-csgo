//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace DotNetPlug
//{
//    [ComVisible(true)]
//    internal static class TypeHelper
//    {
//        //public static ExpandoObject ExpandoNew()
//        //{
//        //    return new ExpandoObject();
//        //}

//        //public static void ExpandoAdd(ExpandoObject obj, string name, object value)
//        //{
//        //    ((IDictionary<string, object>)obj).Add(name, value);
//        //}

//        //public static Dictionary<string, object> ExpandoNew()
//        //{
//        //    return new Dictionary<string, object>();
//        //}

//        //public static void ExpandoAdd(Dictionary<string, object> obj, string name, object value)
//        //{
//        //    obj.Add(name, value);
//        //}

//        public static EventArgBuilder ExpandoNew()
//        {
//            return new EventArgBuilder();
//        }

//        public static void ExpandoAdd(EventArgBuilder obj, string name, object value)
//        {
//            obj.Add(name, value);
//        }
//    }

//    [MarshalAs(UnmanagedType.Struct)]
//    [StructLayout(LayoutKind.Sequential)]
//    public sealed class ExpandoAddData
//    {
//        public EventArgBuilder Obj { get; set; }
//        public string Name { get; set; }

//    }
//}
