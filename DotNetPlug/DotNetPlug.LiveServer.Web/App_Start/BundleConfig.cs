using System.Web;
using System.Web.Optimization;

namespace DotNetPlug.LiveServer.Web
{
    public class BundleConfig
    {
        // Pour plus d'informations sur le regroupement, visitez http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            ScriptBundle scriptAll = (ScriptBundle)(new ScriptBundle("~/Scripts/all.js")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.signalR-{version}.js")
                .Include("~/Scripts/bootstrap.*")
                .Include("~/Scripts/angular.*")
                .Include("~/Scripts/angular-ui-router.*")
                .Include("~/Scripts/angular-sanitize.*")
                .Include("~/Scripts/angular-animate.*")
                .Include("~/Scripts/angular-signalr-hub.*")
                .Include("~/Scripts/angular-ui/ui-bootstrap.*")

                .Include("~/Scripts/liveserver.*")
                );
            bundles.Add(scriptAll);

            StyleBundle styleAll = (StyleBundle)(new StyleBundle("~/Content/all.css")
                .Include("~/Content/bootstrap.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/bootstrap-theme.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/site.*", new CssRewriteUrlTransform())
                .Include("~/Content/css/font-awesome.*", new CssRewriteUrlTransform())
                .Include("~/Content/Icons.*", new CssRewriteUrlTransform())
                );
            bundles.Add(styleAll);
        }
    }
}
