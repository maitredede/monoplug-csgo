//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MonoPlug
//{
//#if DEBUG
//    [System.Diagnostics.DebuggerDisplay("ConVarString : {Name} = {Value}")]
//#endif
//    [Obsolete("To be removed", true)]
//    public sealed class ClsConVarStrings : ClsConvar
//    {
//        internal ClsConVarStrings(ClsMain main, UInt64 nativeId, string name, string description, FCVAR flags)
//            : base(main, nativeId, name, description, flags)
//        {
//        }

//        public string Value
//        {
//            get
//            {
//                string v = null;
//                this._main.InterThreadCall(() =>
//                {

//                    //Get from native var
//                    v = NativeMethods.Mono_GetConVarStringValue(base.NativeID);
//                });
//                return v;
//            }
//            set
//            {
//                //Set native var
//                this._main.InterThreadCall(() =>
//                {
//                    NativeMethods.Mono_SetConVarStringValue(base.NativeID, value);
//                });
//            }
//        }
//    }
//}
