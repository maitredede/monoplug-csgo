using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.Managed
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
            this.Engine.LevelInit += this.Engine_LevelInit;
            this.Engine.LevelShutdown += Engine_LevelShutdown;
            this.Engine.ServerActivate += Engine_ServerActivate;

            this.Engine.ClientActive += Engine_ClientActive;
            this.Engine.ClientCommand += Engine_ClientCommand;
            this.Engine.ClientConnect += Engine_ClientConnect;
            this.Engine.ClientDisconnect += Engine_ClientDisconnect;
            this.Engine.ClientPutInServer += Engine_ClientPutInServer;
            this.Engine.ClientSettingsChanged += Engine_ClientSettingsChanged;
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
