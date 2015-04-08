#ifndef _DOTNETPLUG_GAME_EVENT_H_
#define _DOTNETPLUG_GAME_EVENT_H_
#ifdef _WIN32
#pragma once
#endif

//Source : https://wiki.alliedmods.net/Counter-Strike:_Global_Offensive_Events

typedef enum{
	None = 0,
	player_death = 1,
	player_hurt = 2,

	round_start = 53,
	MAX = 138
} GameEvent;

//const char** g_CSGO_EventNames = {
extern const char* const g_CSGO_EventNames[];

#endif //_DOTNETPLUG_GAME_EVENT_H_