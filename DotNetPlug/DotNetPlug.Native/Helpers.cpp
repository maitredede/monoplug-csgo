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
		/*hr = SET_STRING_PARAM(vtPsa->parray, &i, argv[i]);
		if (FAILED(hr))
		{
		break;
		}*/
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
	if (FAILED(hr))
	{
		SafeArrayDestroy(vtPsa->parray);
	}
	return hr;
}

HRESULT SET_BOOL_PARAM(SAFEARRAY* psa, LONG* i, bool bVal)
{
	HRESULT hr;

	VARIANT item;
	VariantInit(&item);
	item.vt = VT_BOOL;
	item.boolVal = bVal;
	hr = SafeArrayPutElement(psa, i, &item); // insert the string from argv[i] into the safearray
	return hr;
}

HRESULT SET_INT_PARAM(SAFEARRAY* psa, LONG* i, int iVal)
{
	HRESULT hr;

	VARIANT item;
	VariantInit(&item);
	item.vt = VT_INT;
	item.intVal = iVal;
	hr = SafeArrayPutElement(psa, i, &item); // insert the string from argv[i] into the safearray
	return hr;
}

HRESULT SET_STRING_PARAM(SAFEARRAY* psa, LONG* i, const char* pStr)
{
	HRESULT hr;

	VARIANT item;
	VariantInit(&item);
	item.vt = VT_BSTR;
	item.bstrVal = _com_util::ConvertStringToBSTR(pStr);
	hr = SafeArrayPutElement(psa, i, &item); // insert the string from argv[i] into the safearray
	SysFreeString(item.bstrVal);
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

HRESULT CREATE_INSTANCE(_AssemblyPtr spAssembly, const char* className, VARIANT* vtInstance)
{
	bstr_t bstrClassName(className);
	HRESULT hr = spAssembly->CreateInstance(bstrClassName, vtInstance);
	if (FAILED(hr))
	{
		META_LOG(g_PLAPI, "Can't create instance of %s\n", className);
		vtInstance = NULL;
	}
	return hr;
}
//
//HRESULT SET_EXPANDO_STRING_FROM_EVENT_SHORT(variant_t vtExpando, IGameEvent *event, const char* name)
//{
//	int value = event->GetInt(name);
//
//	HRESULT hr;
//	long i = 0;
//
//	//SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
//	//SAFEARRAY* args = SafeArrayCreateVector(VT_UNKNOWN, 0, 3);
//	//SAFEARRAY* args = SafeArrayCreateVector(VT_EMPTY, 0, 3);
//	hr = SafeArrayPutElement(args, &i, vtExpando);
//	i = 1;
//	hr = SET_STRING_PARAM(args, &i, name);
//	i = 2;
//	hr = SET_INT_PARAM(args, &i, value);
//
//	hr = g_Managed.m_Method_DotNetPlug_TypeHelper_ExpandoAdd->Invoke_3(vtExpando, args, NULL);
//
//	hr = SafeArrayDestroy(args);
//
//	return hr;
//}
//
//HRESULT SET_EXPANDO_STRING_FROM_EVENT_STRING(variant_t vtExpando, IGameEvent *event, const char* name)
//{
//	const char* value = event->GetString(name);
//
//	HRESULT hr;
//	long i = 0;
//
//	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
//	hr = SafeArrayPutElement(args, &i, vtExpando);
//	i = 1;
//	hr = SET_STRING_PARAM(args, &i, name);
//	i = 2;
//	hr = SET_STRING_PARAM(args, &i, value);
//
//	hr = g_Managed.m_Method_DotNetPlug_TypeHelper_ExpandoAdd->Invoke_3(vtExpando, args, NULL);
//
//	hr = SafeArrayDestroy(args);
//
//	return hr;
//}
//
//HRESULT SET_EXPANDO_STRING_FROM_EVENT_BOOL(variant_t vtExpando, IGameEvent *event, const char* name)
//{
//	bool value = event->GetBool(name);
//
//	HRESULT hr;
//	long i = 0;
//
//	SAFEARRAY* args = SafeArrayCreateVector(VT_VARIANT, 0, 3);
//	hr = SafeArrayPutElement(args, &i, vtExpando);
//	i = 1;
//	hr = SET_STRING_PARAM(args, &i, name);
//	i = 2;
//	hr = SET_BOOL_PARAM(args, &i, value);
//
//	hr = g_Managed.m_Method_DotNetPlug_TypeHelper_ExpandoAdd->Invoke_3(vtExpando, args, NULL);
//
//	hr = SafeArrayDestroy(args);
//
//	return hr;
//}

HRESULT GET_TYPE_FUNC(_AssemblyPtr pAssembly, const char* sType, _Type** pType)
{
	bstr_t bstrType(sType);
	HRESULT hr = pAssembly->GetType_2(bstrType, pType);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to get type %s type w/hr 0x%08lx\n", sType, hr);
	}
	return hr;
}

HRESULT LOAD_ASSEMBLY_FUNC(_AppDomainPtr pAppDomain, const char* sAssemblyName, _Assembly** pAssembly)
{
	bstr_t bstrMscorlibAssemblyName(sAssemblyName);
	HRESULT hr = pAppDomain->Load_2(bstrMscorlibAssemblyName, pAssembly);
	if (FAILED(hr)){
		META_CONPRINTF("Failed to load the assembly %s w/hr 0x%08lx\n", sAssemblyName, hr);
	}
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