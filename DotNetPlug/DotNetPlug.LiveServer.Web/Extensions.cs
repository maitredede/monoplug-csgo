using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DotNetPlug.LiveServer.Web
{
    public static class Extensions
    {
        public static MvcHtmlString Json(this HtmlHelper html, object obj)
        {
            MvcHtmlString str = new MvcHtmlString(Newtonsoft.Json.JsonConvert.ToString(obj));
            return str;
        }
    }
}