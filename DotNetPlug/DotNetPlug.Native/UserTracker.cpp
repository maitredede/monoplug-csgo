#include "UserTracker.h"
#include "Plugin.h"

//---------------------------------------------------------------------------------
// Purpose: FindPlayerByIndex
//---------------------------------------------------------------------------------
bool FindPlayerByIndex(player_t *player_ptr)
{

	if (player_ptr->index < 1 || player_ptr->index > g_DotNetPlugPlugin.max_players)
	{
		return false;
	}

	edict_t *pEntity = PEntityOfEntIndex(player_ptr->index);
	if (pEntity && !pEntity->IsFree())
	{
		IPlayerInfo *playerinfo = playerinfomanager->GetPlayerInfo(pEntity);
		if (playerinfo && playerinfo->IsConnected())
		{
			if (playerinfo->IsHLTV()) return false;
			player_ptr->player_info = playerinfo;
			player_ptr->team = playerinfo->GetTeamIndex();
			player_ptr->user_id = playerinfo->GetUserID();
			Q_strcpy(player_ptr->name, playerinfo->GetName());
			Q_strcpy(player_ptr->steam_id, playerinfo->GetNetworkIDString());
			player_ptr->health = playerinfo->GetHealth();
			player_ptr->is_dead = playerinfo->IsObserver() | playerinfo->IsDead();
			player_ptr->entity = pEntity;

			if (FStrEq(player_ptr->steam_id, "BOT"))
			{
				if (g_DotNetPlugPlugin.tv_name && strcmp(player_ptr->name, g_DotNetPlugPlugin.tv_name->GetString()) == 0)
				{
					return false;
				}

				Q_strcpy(player_ptr->ip_address, "");
				player_ptr->is_bot = true;
			}
			else
			{
				player_ptr->is_bot = false;
				GetIPAddressFromPlayer(player_ptr);
			}
			return true;
		}
	}

	return false;
}

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
void	UserTracker::Load(void)
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
void	UserTracker::Unload(void)
{
	for (int i = 0; i < 65536; i++)
	{
		hash_table[i] = -1;
	}
}

//---------------------------------------------------------------------------------
// Purpose: Level Loaded
//---------------------------------------------------------------------------------
void	UserTracker::LevelInit(void)
{
	for (int i = 0; i < 65536; i++)
	{
		hash_table[i] = -1;
	}
}

//---------------------------------------------------------------------------------
// Purpose: Client Active
//---------------------------------------------------------------------------------
void	UserTracker::ClientActive(edict_t *pEntity)
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