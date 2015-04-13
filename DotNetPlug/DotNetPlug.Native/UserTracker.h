#ifndef _DOTNETPLUG_USER_TRACKER_H_
#define _DOTNETPLUG_USER_TRACKER_H_
#ifdef _WIN32
#pragma once
#endif

#include <edict.h>
#include "Player.h"

class UserTracker {
public:
	UserTracker();
	~UserTracker();

	void		ClientActive(edict_t *pEntity);
	void		ClientDisconnect(player_t *player_ptr);
	void		Load(void);
	void		Unload(void);
	void		LevelInit(void);
	int			GetIndex(int user_id)
	{
		return (int)hash_table[user_id];
	}
	int Count();
	//bool Get(int index, player_t& player);
	void GetAll(player_t* playerArray, int nbr);
private:

	// Hash table of user ids
	char hash_table[65536];
};

extern UserTracker gUserTracker;

#endif //_DOTNETPLUG_USER_TRACKER_H_
