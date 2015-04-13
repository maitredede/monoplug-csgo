#ifndef _DOTNETPLUG_GAME_EVENT_H_
#define _DOTNETPLUG_GAME_EVENT_H_
#ifdef _WIN32
#pragma once
#endif

typedef enum{
	None = 0,
	//CSGO :  https://wiki.alliedmods.net/Counter-Strike:_Global_Offensive_Events
	player_death = 1,
	player_hurt = 2,
	item_purchase = 3,
	bomb_beginplant = 4,

	bomb_abortplant = 5,
	bomb_planted = 6,
	bomb_defused = 7,
	bomb_exploded = 8,
	bomb_dropped = 9,

	bomb_pickup = 10,
	defuser_dropped = 11,
	defuser_pickup = 12,
	announce_phase_end = 13,
	cs_intermission = 14,

	bomb_begindefuse = 15,
	bomb_abortdefuse = 16,
	hostage_follows = 17,
	hostage_hurt = 18,
	hostage_killed = 19,

	hostage_rescued = 20,
	hostage_stops_following = 21,
	hostage_rescued_all = 22,
	hostage_call_for_help = 23,
	vip_escaped = 24,

	vip_killed = 25,
	player_radio = 26,
	bomb_beep = 27,
	weapon_fire = 28,
	weapon_fire_on_empty = 29,

	weapon_outofammo = 30,
	weapon_reload = 31,
	weapon_zoom = 32,
	silencer_detach = 33,
	inspect_weapon = 34,

	weapon_zoom_rifle = 35,
	player_spawned = 36,
	item_pickup = 37,
	ammo_pickup = 38,
	item_equip = 39,

	enter_buyzone = 40,
	exit_buyzone = 41,
	buytime_ended = 42,
	enter_bombzone = 43,
	exit_bombzone = 44,

	enter_rescue_zone = 45,
	exit_rescue_zone = 46,
	silencer_off = 47,
	silencer_on = 48,
	buymenu_open = 49,

	buymenu_close = 50,
	round_prestart = 51,
	round_poststart = 52,
	round_start = 53,
	round_end = 54,

	grenade_bounce = 55,
	hegrenade_detonate = 56,
	flashbang_detonate = 57,
	smokegrenade_detonate = 58,
	smokegrenade_expired = 59,

	molotov_detonate = 60,
	decoy_detonate = 61,
	decoy_started = 62,
	inferno_startburn = 63,
	inferno_expire = 64,

	inferno_extinguish = 65,
	decoy_firing = 66,
	bullet_impact = 67,
	player_footstep = 68,
	player_jump = 69,

	player_blind = 70,
	player_falldamage = 71,
	door_moving = 72,
	round_freeze_end = 73,
	mb_input_lock_success = 74,

	mb_input_lock_cancel = 75,
	nav_blocked = 76,
	nav_generate = 77,
	player_stats_updated = 78,
	achievement_info_loaded = 79,

	spec_target_updated = 80,
	spec_mode_updated = 81,
	hltv_changed_mode = 82,
	cs_game_disconnected = 83,
	cs_win_panel_round = 84,

	cs_win_panel_match = 85,
	cs_match_end_restart = 86,
	cs_pre_restart = 87,
	show_freezepanel = 88,
	hide_freezepanel = 89,

	freezecam_started = 90,
	player_avenged_teammate = 91,
	achievement_earned = 92,
	achievement_earned_local = 93,
	item_found = 94,

	item_gifted = 95,
	repost_xbox_achievements = 96,
	match_end_conditions = 97,
	round_mvp = 98,
	player_decal = 99,

	teamplay_round_start = 100,
	client_disconnect = 101,
	gg_player_levelup = 102,
	ggtr_player_levelup = 103,
	ggprogressive_player_levelup = 104,

	gg_killed_enemy = 105,
	gg_final_weapon_achieved = 106,
	gg_bonus_grenade_achieved = 107,
	switch_team = 108,
	gg_leader = 109,

	gg_player_impending_upgrade = 110,
	write_profile_data = 111,
	trial_time_expired = 112,
	update_matchmaking_stats = 113,
	player_reset_vote = 114,

	enable_restart_voting = 115,
	sfuievent = 116,
	start_vote = 117,
	player_given_c4 = 118,
	gg_reset_round_start_sounds = 119,

	tr_player_flashbanged = 120,
	tr_highlight_ammo = 121,
	tr_mark_complete = 122,
	tr_mark_best_time = 123,
	tr_exit_hint_trigger = 124,

	bot_takeover = 125,
	tr_show_finish_msgbox = 126,
	tr_show_exit_msgbox = 127,
	reset_player_controls = 128,
	jointeam_failed = 129,

	teamchange_pending = 130,
	material_default_complete = 131,
	cs_prev_next_spectator = 132,
	cs_handle_ime_event = 133,
	nextlevel_changed = 134,

	seasoncoin_levelup = 135,
	tournament_reward = 136,
	start_halftime = 137,

	//GenericSourceServer : https://wiki.alliedmods.net/Generic_Source_Server_Events
	server_spawn,
	server_shutdown,
	server_cvar,
	server_message,
	server_addban,
	server_removeban,
	player_connect,
	player_info,
	player_disconnect,
	player_activate,
	player_say,

	//Generic source events : https://wiki.alliedmods.net/Generic_Source_Events
	team_info,
	team_score,
	teamplay_broadcast_audio,
	player_team,
	player_class,
	player_death,
	player_hurt,
	player_chat,
	player_score,
	player_spawn,
	player_shoot,
	player_use,
	player_changename,
	player_hintmessage,
	base_player_teleported,
	game_init,


	MAX,
} GameEvent;
//const char** g_CSGO_EventNames = {
extern const char* const g_CSGO_EventNames[];

#endif //_DOTNETPLUG_GAME_EVENT_H_