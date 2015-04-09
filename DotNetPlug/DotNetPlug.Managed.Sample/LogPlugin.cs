using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug
{
    public sealed class LogPlugin : PluginBase
    {
        private async Task Log(string msg)
        {
            await this.Engine.Log("Log: {0}", msg);
        }

        public override async Task Load()
        {
            await this.Log("Load");
            //this.Engine.LevelInit += this.Engine_LevelInit;
            //this.Engine.LevelShutdown += Engine_LevelShutdown;
            //this.Engine.ServerActivate += Engine_ServerActivate;

            //this.Engine.ClientActive += Engine_ClientActive;
            //this.Engine.ClientCommand += Engine_ClientCommand;
            //this.Engine.ClientConnect += Engine_ClientConnect;
            //this.Engine.ClientDisconnect += Engine_ClientDisconnect;
            //this.Engine.ClientPutInServer += Engine_ClientPutInServer;
            //this.Engine.ClientSettingsChanged += Engine_ClientSettingsChanged;
            //this.Engine.GameEvent += Engine_GameEvent;
        }

        private async void Engine_GameEvent(object sender, GameEventEventArgs e)
        {
            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.Converters.Add(new GameEventDataFlatConverter());
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(e, Newtonsoft.Json.Formatting.None, settings);
            GameEventEventArgs e2 = Newtonsoft.Json.JsonConvert.DeserializeObject<GameEventEventArgs>(json, settings);
            string line = string.Format("GameEvent : {0}={1}", e.Event, json);
            await this.Log(line);
            Console.WriteLine(line);
        }

        private async void Engine_ServerActivate(object sender, ServerActivateEventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_LevelShutdown(object sender, EventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_ClientSettingsChanged(object sender, EventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_ClientPutInServer(object sender, EventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_ClientDisconnect(object sender, EventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_ClientConnect(object sender, EventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_ClientCommand(object sender, EventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_ClientActive(object sender, EventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        private async void Engine_LevelInit(object sender, LevelInitEventArgs e)
        {
            await this.Log(System.Reflection.MethodInfo.GetCurrentMethod().Name);
        }

        public override async Task Unload()
        {
            await this.Log("Unload");
        }
    }
}
