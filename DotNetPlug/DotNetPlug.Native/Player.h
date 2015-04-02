#ifndef _DOTNETPLUG_PLAYER_H_
#define _DOTNETPLUG_PLAYER_H_

# include <edict.h>
# include <iplayerinfo.h>

struct player_t
{
	char	steam_id[MAX_NETWORKID_LENGTH];
	char	ip_address[128];
	char	name[MAX_PLAYER_NAME_LENGTH];
	char	password[128];
	int		user_id;
	int		team;
	int		health;
	int		index;
	edict_t	*entity;
	bool	is_bot;
	bool	is_dead;
	IPlayerInfo *player_info;
};

#endif //_DOTNETPLUG_PLAYER_H_