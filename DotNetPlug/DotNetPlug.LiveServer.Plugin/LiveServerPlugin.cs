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

        private readonly GameEvent[] m_eventsToIgnore;
        private HubConnection m_hubConnection;
        private IHubProxy m_PluginHubProxy;
        private IPluginHubServer m_PluginHub;

        public LiveServerPlugin()
        {
            this.m_eventsToIgnore = new GameEvent[]{
                //Noisy
                GameEvent.player_footstep,
                GameEvent.bullet_impact,
              
                //Not used in this case
                GameEvent.achievement_earned,
                GameEvent.achievement_earned_local,
                GameEvent.achievement_info_loaded,
            };
        }

        public override async Task Load()
        {
            string hostUrl = this.GetAppSettings("LiveServer:HostUrl");
            string serverId = this.GetAppSettings("LiveServer:ServerId");
            string serverKey = this.GetAppSettings("LiveServer:ServerKey");

            this.m_hubConnection = new HubConnection(hostUrl, useDefaultUrl: true);
            this.m_PluginHubProxy = this.m_hubConnection.CreateHubProxy("PluginServerHub");
            this.m_hubConnection.Error += this.hubConnection_Error;
            this.m_PluginHub = new PluginHubClient(this.m_PluginHubProxy);

            //this.m_hubConnection.CreateHubProxy
            //IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("StockTickerHub");
            //stockTickerHubProxy.On<Stock>("UpdateStockPrice", stock => Console.WriteLine("Stock update for {0} new price {1}", stock.Symbol, stock.Price));
            //await hubConnection.Start(); throw new NotImplementedException();
            await this.m_hubConnection.Start();

            if (await this.m_PluginHub.Hello(serverId, serverKey))
            {
                this.Engine.GameEvent += this.Engine_GameEvent;
                await this.Engine.Log("LiveServer : Ready :)");
            }
            else
            {
                await this.Engine.Log("LiveServer : server id/key error");
            }
            PlayerData[] players = await this.Engine.GetPlayers();
            await this.m_PluginHub.SetPlayers(players);
        }

        private void hubConnection_Error(Exception obj)
        {

        }

        private async void Engine_GameEvent(object sender, GameEventEventArgs e)
        {
            if (this.m_eventsToIgnore.Contains(e.Event))
                return;
            switch (e.Event)
            {
                case GameEvent.switch_team:
                case GameEvent.teamchange_pending:
                case GameEvent.player_spawn:
                case GameEvent.player_spawned:
                case GameEvent.player_team:
                case GameEvent.player_death:
                case GameEvent.player_disconnect:
                case GameEvent.player_connect:
                    await this.m_PluginHub.SetPlayers(await this.Engine.GetPlayers());
                    break;
            }
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
