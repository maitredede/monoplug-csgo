using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPlug.LiveServer
{
    public sealed class LiveServerPlugin : PluginBase
    {
        static LiveServerPlugin()
        {
            Tools.ConfigureSignalrSerializer();
        }

        private HubConnection m_hubConnection;
        private IHubProxy m_PluginHubProxy;
        private IPluginHubServer m_PluginHub;

        public override async Task Load()
        {
            this.m_hubConnection = new HubConnection("http://localhost:54908/", useDefaultUrl: true);
            this.m_PluginHubProxy = this.m_hubConnection.CreateHubProxy("PluginServerHub");
            this.m_PluginHub = new PluginHubClient(this.m_PluginHubProxy);

            //this.m_hubConnection.CreateHubProxy
            //IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("StockTickerHub");
            //stockTickerHubProxy.On<Stock>("UpdateStockPrice", stock => Console.WriteLine("Stock update for {0} new price {1}", stock.Symbol, stock.Price));
            //await hubConnection.Start(); throw new NotImplementedException();
            await this.m_hubConnection.Start();

            if (await this.m_PluginHub.Hello("Demo", "DemoKey"))
            {
                this.Engine.GameEvent += this.Engine_GameEvent;
                await this.Engine.Log("LiveServer : Ready :)");
            }
            else
            {
                await this.Engine.Log("LiveServer : server id/key error");
            }
        }

        private async void Engine_GameEvent(object sender, GameEventEventArgs e)
        {
            await this.m_PluginHub.RaiseEvent(e.ToData());
        }

        public override Task Unload()
        {
            this.m_hubConnection.Stop();
            this.m_hubConnection.Dispose();
            return Task.FromResult(0);
        }
    }
}
