using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(DotNetPlug.LiveServer.Web.Startup))]

namespace DotNetPlug.LiveServer.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            HubConfiguration hubConfiguration = new HubConfiguration()
            {
                EnableDetailedErrors = true,
                EnableJavaScriptProxies = true,
                //EnableJSONP = false,
            };

            //app.MapSignalR(hubConfiguration);
            Tools.ConfigureSignalrSerializer();
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                //var hubConfiguration = new HubConfiguration 
                //{
                //    // You can enable JSONP by uncommenting line below.
                //    // JSONP requests are insecure but some older browsers (and some
                //    // versions of IE) require JSONP to work cross domain
                //    // EnableJSONP = true
                //};
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}