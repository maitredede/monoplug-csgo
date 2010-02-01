//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace MonoPlug
//{
//    partial class ClsPluginBase
//    {
//        /// <summary>
//        /// Register a ConCommand
//        /// </summary>
//        /// <param name="name">Name of command</param>
//        /// <param name="help">Help text of command</param>
//        /// <param name="flags">Flags of command</param>
//        /// <param name="code">Code to invoke</param>
//        /// <param name="completion">Auto-completion of command</param>
//        /// <param name="async">True if command will be called asynchronously</param>
//        /// <returns>ConCommand instance if success, else null</returns>
//        public ClsConCommand RegisterConCommand(string name, string help, FCVAR flags, ConCommandDelegate code, ConCommandCompleteDelegate completion, bool async)
//        {
//            return this._conItem.RegisterConCommand(this, name, help, flags, code, completion, async);
//        }

//        /// <summary>
//        /// Unregister a ConCommand
//        /// </summary>
//        /// <param name="command">ConCommand instance to unregister</param>
//        /// <returns>True if unregister is successfull</returns>
//        public void UnregisterConCommand(ClsConCommand command)
//        {
//            this._main.UnregisterConCommand(this, command);
//        }
//    }
//}
