using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MonoPlug
{
    partial class ClsMain
    {
        private void clr_versions(string line, string[] args)
        {
            this._msg.Msg("Mono version : {0}\n", ClsMain.GetMonoVersion());
            try
            {
                this.WriteAssemblyVersion(typeof(MySql.Data.MySqlClient.MySqlConnection), "MySQL version : {0}\n");
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
            }
            try
            {
                this.WriteAssemblyVersion(typeof(ICSharpCode.SharpZipLib.Zip.ZipEntry), "SharpZip version : {0}\n");
            }
            catch (Exception ex)
            {
                this._msg.Warning(ex);
            }
        }

        private void WriteAssemblyVersion(Type type, string format)
        {
            try
            {
                object[] arr = type.Assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), true);
                if (arr != null && arr.Length > 0)
                {
                    foreach (AssemblyVersionAttribute att in arr)
                    {
                        this._msg.Msg(format, att.Version);
                    }
                }
                else
                {
                    this._msg.Msg(format, "no version");
                }
            }
            catch (Exception ex)
            {
                this._msg.Warning(format, ex.Message);
            }
        }
    }
}
