#include "EventManager.h"
#include "Managed.h"

EventManager g_EventManager;

EventManager::EventManager()
{

}

EventManager::~EventManager()
{

}

int EventManager::GetEventDebugID(void)
{
	return EVENT_DEBUG_ID_INIT;
}

void EventManager::AddHandler(GameEvent e)
{
	int i = (int)e;
	if (g_CSGO_EventNames[i] && strlen(g_CSGO_EventNames[i]))
	{
		gameevents->AddListener(this, g_CSGO_EventNames[i], true);
	}
	else{
		DebugMsg("Can't handle event");
	}
}

void EventManager::AttachEvents()
{
	/*for (int i = None; i < MAX; i++){
		if (g_CSGO_EventNames[i] && strlen(g_CSGO_EventNames[i])){
		gameevents->AddListener(this, g_CSGO_EventNames[i], true);
		}
		}*/
	this->AddHandler(player_hurt);
	this->AddHandler(player_team);
	this->AddHandler(player_death);
	this->AddHandler(player_say);
	this->AddHandler(player_spawn);

	this->AddHandler(weapon_fire);
	this->AddHandler(hostage_stops_following);
	this->AddHandler(bomb_planted);
	this->AddHandler(bomb_dropped);
	this->AddHandler(bomb_exploded);
	this->AddHandler(bomb_defused);
	this->AddHandler(bomb_begindefuse);
	this->AddHandler(bomb_pickup);
	this->AddHandler(hostage_rescued);
	this->AddHandler(hostage_follows);
	this->AddHandler(hostage_killed);
	//this->AddHandler(round_start);
	this->AddHandler(round_end);
	this->AddHandler(round_freeze_end);
	this->AddHandler(vip_escaped);
	this->AddHandler(vip_killed);

	this->AddHandler(enter_buyzone);
	this->AddHandler(exit_buyzone);
	this->AddHandler(enter_bombzone);
	this->AddHandler(exit_bombzone);

	this->AddHandler(game_init);
	this->AddHandler(game_end);
	this->AddHandler(game_message);
	this->AddHandler(game_newmap);
	this->AddHandler(game_start);

	this->AddHandler(player_spawned);
	this->AddHandler(switch_team);
	this->AddHandler(player_team);
	this->AddHandler(player_connect);
	this->AddHandler(player_changename);
	this->AddHandler(player_chat);
	this->AddHandler(player_given_c4);
	this->AddHandler(player_disconnect);
	this->AddHandler(buytime_ended);

	gameevents->AddListener(this, "round_officially_ended", true);
	gameevents->AddListener(this, "round_start", true);
	gameevents->AddListener(this, "round_end", true);
	gameevents->AddListener(this, "teamplay_round_start", true);
}

void EventManager::DetachEvents()
{
	gameevents->RemoveListener(this);
}

void EventManager::FireGameEvent(IGameEvent *event)
{
	const char* name = event->GetName();
	bool found = false;
	GameEvent e = None;
	for (int i = None; i < MAX; i++)
	{
		if (g_CSGO_EventNames[i] && strlen(g_CSGO_EventNames[i]) > 0)
		{
			if (strcmp(g_CSGO_EventNames[i], name) == 0){
				found = true;
				e = (GameEvent)i;
				break;
			}
		}
	}
	if (found)
	{
		g_Managed.RaiseGameEvent(e, event);
	}
	else
	{
		DebugMsg("Event not found");
	}
}