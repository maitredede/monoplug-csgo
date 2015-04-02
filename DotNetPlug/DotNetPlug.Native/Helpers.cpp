#include "Helpers.h"
#include "Plugin.h"
#include <inetchannelinfo.h>

#ifdef MANAGED_WIN32
HRESULT SET_CALLBACK(SAFEARRAY* params, long idx, LONGLONG funcPtr)
{
	HRESULT hr;
	variant_t params0;
	params0.llVal = funcPtr;
	params0.vt = VT_I8;
	hr = SafeArrayPutElement(params, &idx, &params0);
	return hr;
};

HRESULT CREATE_STRING_ARRAY(int argc, const char** argv, VARIANT* vtPsa)
{
	long i;
	HRESULT hr;

	vtPsa->vt = (VT_ARRAY | VT_BSTR);
	vtPsa->parray = SafeArrayCreateVector(VT_BSTR, 0, argc); // create an array of strings

	for (i = 0; i < argc; i++)
	{
		VARIANT item;
		VariantInit(&item);
		item.vt = VT_BSTR;
		item.bstrVal = _com_util::ConvertStringToBSTR(argv[i]);
		hr = SafeArrayPutElement(vtPsa->parray, &i, item.bstrVal); // insert the string from argv[i] into the safearray
		if (FAILED(hr))
		{
			SafeArrayDestroy(vtPsa->parray);
			SysFreeString(item.bstrVal);
			return hr;
		}
		SysFreeString(item.bstrVal);
	}
	return hr;
}

HRESULT CREATE_STRING_ARRAY_ARGS(int argc, const char** argv, long paramIndex, SAFEARRAY** params)
{
	HRESULT hr;
	long i = paramIndex;
	*params = SafeArrayCreateVector(VT_VARIANT, 0, 1);
	VARIANT vtPsa;
	hr = CREATE_STRING_ARRAY(argc, argv, &vtPsa);
	hr = SafeArrayPutElement(*params, &i, &vtPsa);
	return hr;
}
#endif //MANAGED_WIN32

bool FindPlayerByEntity(player_t* player_ptr)
{
	if (player_ptr->entity && !player_ptr->entity->IsFree())
	{
		IPlayerInfo *playerinfo = playerinfomanager->GetPlayerInfo(player_ptr->entity);
		if (playerinfo && playerinfo->IsConnected())
		{
			if (playerinfo->IsHLTV()) return false;
			player_ptr->player_info = playerinfo;
			player_ptr->index = IndexOfEdict(player_ptr->entity);
			player_ptr->user_id = playerinfo->GetUserID();
			player_ptr->team = playerinfo->GetTeamIndex();
			player_ptr->health = playerinfo->GetHealth();
			player_ptr->is_dead = playerinfo->IsObserver() | playerinfo->IsDead();
			Q_strcpy(player_ptr->name, playerinfo->GetName());
			Q_strcpy(player_ptr->steam_id, playerinfo->GetNetworkIDString());

			if (FStrEq(player_ptr->steam_id, "BOT"))
			{
				if (g_DotNetPlugPlugin.tv_name && strcmp(player_ptr->name, g_DotNetPlugPlugin.tv_name->GetString()) == 0)
				{
					return false;
				}

				player_ptr->is_bot = true;
				Q_strcpy(player_ptr->ip_address, "");
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

bool FStrEq(const char* str1, const char* str2)
{
	return(Q_stricmp(str1, str2) == 0);
}

void GetIPAddressFromPlayer(player_t *player)
{
	INetChannelInfo *nci = engine->GetPlayerNetInfo(player->index);

	if (!nci)
	{
		Q_strcpy(player->ip_address, "");
		return;
	}

	const char *ip_address = nci->GetAddress();

	if (ip_address)
	{
		int str_length = Q_strlen(ip_address);
		for (int i = 0; i <= str_length; i++)
		{
			// Strip port id
			if (ip_address[i] == ':')
			{
				player->ip_address[i] = '\0';
				return;
			}

			player->ip_address[i] = ip_address[i];
		}
	}
	else
	{
		Q_strcpy(player->ip_address, "");
	}

	return;
}
