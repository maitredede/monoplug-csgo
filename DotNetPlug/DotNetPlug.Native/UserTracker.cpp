#include "UserTracker.h"
#include "Plugin.h"

UserTracker::UserTracker()
{
	//hash_table = (unsigned char *) malloc(sizeof(unsigned char) * 65536);
	// Setup hash table for weapon search speed improvment
	for (int i = 0; i < 65536; i++)
	{
		hash_table[i] = -1;
	}
}

UserTracker::~UserTracker()
{
	// Cleanup
	//	free(hash_table);
}

//---------------------------------------------------------------------------------
// Purpose: Plugin Loaded
//---------------------------------------------------------------------------------
void UserTracker::Load(void)
{
	for (int i = 0; i < 65536; i++)
	{
		hash_table[i] = -1;
	}

	for (int i = 1; i <= g_DotNetPlugPlugin.max_players; i++)
	{
		player_t player;

		player.index = i;
		if (!FindPlayerByIndex(&player)) continue;
		hash_table[player.user_id] = i;
	}
}

//---------------------------------------------------------------------------------
// Purpose: Plugin Unloaded
//---------------------------------------------------------------------------------
void UserTracker::Unload(void)
{
	for (int i = 0; i < 65536; i++)
	{
		hash_table[i] = -1;
	}
}

//---------------------------------------------------------------------------------
// Purpose: Level Loaded
//---------------------------------------------------------------------------------
void UserTracker::LevelInit(void)
{
	for (int i = 0; i < 65536; i++)
	{
		hash_table[i] = -1;
	}
}

//---------------------------------------------------------------------------------
// Purpose: Client Active
//---------------------------------------------------------------------------------
void UserTracker::ClientActive(edict_t *pEntity)
{
	if (pEntity && !pEntity->IsFree())
	{
		IPlayerInfo *playerinfo = playerinfomanager->GetPlayerInfo(pEntity);
		if (playerinfo && playerinfo->IsConnected())
		{
			hash_table[playerinfo->GetUserID()] = IndexOfEdict(pEntity);
		}
	}
}

//---------------------------------------------------------------------------------
// Purpose: Check Player on disconnect
//---------------------------------------------------------------------------------
void UserTracker::ClientDisconnect(player_t	*player_ptr)
{
	hash_table[player_ptr->user_id] = -1;
}

int UserTracker::Count()
{
	int count = 0;
	for (int i = 0; i < 65536; i++)
	{
		if (hash_table[i] != -1){
			count++;
		}
	}
	return count;
}

//const player_t* UserTracker::Get(int index)
void UserTracker::GetAll(player_t* playerArray, int nbr)
{
	int pCount = 0;
	for (int i = 0; i < 65536; i++)
	{
		if (hash_table[i] != -1)
		{
			playerArray[pCount].index = hash_table[i];

			if (FindPlayerByIndex(&(playerArray[pCount])))
			{
				pCount++;
			}
		}
	}
}
