using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DotNetPlug.LiveServer.Web.Controllers
{
    public class ScriptGenController : Controller
    {
        public ActionResult LiveServerData()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"""use strict""; angular.module(""liveserver"")");
            this.AddEnumStringValue(sb, typeof(GameEvent));
            //this.AddEnumIntValue(sb, typeof(GameEvent));
            return this.JavaScript(sb.ToString());
        }

        private void AddEnumStringValue(StringBuilder sb, Type tEnum, string dataName = null)
        {
            sb.AppendFormat(@".constant(""{0}"", ", dataName ?? tEnum.Name);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var value in Enum.GetValues(tEnum))
            {
                dic.Add(value.ToString(), value.ToString());
            }
            sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(dic));
            sb.Append(")");
        }

        private void AddEnumIntValue(StringBuilder sb, Type tEnum, string dataName = null)
        {
            sb.AppendFormat(@".constant(""{0}"", ", dataName ?? tEnum.Name);
            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach (var value in Enum.GetValues(tEnum))
            {
                dic.Add(value.ToString(), Convert.ToInt32(value));
            }
            sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(dic));
            sb.Append(")");
        }
    }
}