using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.LiveServer
{
    public static class Tools
    {
        public static void ConfigureSignalrSerializer()
        {
            Lazy<Newtonsoft.Json.JsonSerializer> jsonSerializer = new Lazy<Newtonsoft.Json.JsonSerializer>(() =>
            {
                Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.Converters.Add(new GameEventDataFlatConverter());
                Newtonsoft.Json.JsonSerializer jsz = Newtonsoft.Json.JsonSerializer.Create(settings);
                return jsz;
            });
            GlobalHost.DependencyResolver.Register(typeof(Newtonsoft.Json.JsonSerializer), () => jsonSerializer.Value);
        }
    }
}
