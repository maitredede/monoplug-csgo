using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoPlug
{
    partial class ClsPluginBase
    {
        /// <summary>
        /// Register a convar
        /// </summary>
        /// <param name="name">Name of convar</param>
        /// <param name="help">Help text of convar</param>
        /// <param name="flags">Flags of convar</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Convar instance if success, else null.</returns>
        public ClsConvar RegisterConvar(string name, string help, FCVAR flags, string defaultValue)
        {
#if DEBUG
            this._main.DevMsg("ClsConvar::RegisterConvar() in [{0}]\n", AppDomain.CurrentDomain.FriendlyName);
#endif

            ClsConvarMain varMain = this._main.RegisterConvar(this, name, help, flags, defaultValue);
            if (varMain != null)
            {
                ClsConvar var = new ClsConvar(varMain);
                varMain.Init(var);
                return var;
            }
            return null;
        }

        /// <summary>
        /// Unregister a convar
        /// </summary>
        /// <param name="var">Convar instance</param>
        public void UnregisterConvar(ClsConvar var)
        {
            this._main.UnregisterConvar(this, var.ConvarMain);
        }
    }
}
