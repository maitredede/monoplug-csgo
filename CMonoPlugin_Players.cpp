#include "CMonoPlugin.h"
#include <inetchannelinfo.h>

namespace MonoPlugin
{
	//CMonoPlayer* CMonoPlugin::CreatePlayer(int index, int userid, const char* name, const char* networkid, const char* address, bool bot, edict_t* entity)
	//{
	//	if(index < 0)
	//		return NULL;
	//	//int pos = index - 1;

	//	CMonoPlayer* player = new CMonoPlayer(this, userid, index, bot, entity);
	//	player->SetData(name, networkid, address);

	//	this->m_allClients[userid] = player;
	//	return player;
	//}

	//CMonoPlayer* CMonoPlugin::GetPlayerByUserId(int index)
	//{
	//	if(index == -1)
	//	{
	//		return NULL;
	//	}
	//	else
	//	{
	//		return this->m_allClients[index];
	//	}
	//}

	//void CMonoPlugin::RefreshPlayerInfo(int index, const char* name, const char* address, edict_t* pEntity)
	//{
	//	if(!this->m_players[index].MPlayer)
	//	{
	//		if(!pEntity)
	//		{
	//			pEntity = g_engine->PEntityOfEntIndex(index);
	//		}
	//		this->m_players[index].PlayerEdict = pEntity;
	//		this->m_players[index].Userid = g_engine->GetPlayerUserId(this->m_players[index].PlayerEdict);
	//		this->m_players[index].MPlayer = CMonoHelpers::ClassNew(g_Domain, this->m_Class_ClsPlayer);
	//	}

	//	//Bot status
	//	const char *pSteamid = g_engine->GetPlayerNetworkIDString(this->m_players[index].PlayerEdict);
	//	if(strcmp(pSteamid,"BOT") == 0 || strcmp(pSteamid,"HLTV") == 0)
	//		this->m_players[index].IsBot = true;
	//	else 
	//		this->m_players[index].IsBot = false;

	//	//Name
	//	mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_name, CMonoHelpers::GetString(g_Domain, name));

	//	IPlayerInfo* pi = g_PlayerInfoManager->GetPlayerInfo(this->m_players[index].PlayerEdict);
	//	INetChannelInfo* net = g_engine->GetPlayerNetInfo(this->m_players[index].Userid);

	//	float avgLatency = -1.0;
	//	float timeConnected = -1.0;
	//	MonoString* mAddress;

	//	if(net)
	//	{
	//		avgLatency = net->GetAvgLatency(MAX_FLOWS);
	//		timeConnected = net->GetTimeConnected();
	//		mAddress = CMonoHelpers::GetString(g_Domain, net->GetAddress());
	//	}
	//	else
	//	{
	//		if(address)
	//		{
	//			mAddress = CMonoHelpers::GetString(g_Domain, address);
	//		}
	//		else
	//		{
	//			mAddress = NULL;
	//		}
	//	}
	//	int pfrag = -1;
	//	int pdeath = -1;
	//	if(pi)
	//	{
	//		pfrag = pi->GetFragCount();
	//		pdeath = pi->GetDeathCount();
	//	}

	//	MonoString* lng;
	//	if(this->m_players[index].IsBot)
	//	{
	//		lng = CMonoHelpers::GetString(g_Domain, NULL);
	//	}
	//	else
	//	{
	//		lng = CMonoHelpers::GetString(g_Domain, g_engine->GetClientConVarValue(index, "cl_language"));
	//	}

	//	MonoString* mName;
	//	if(pi)
	//	{
	//		mName = CMonoHelpers::GetString(g_Domain, pi->GetName());
	//	}
	//	else
	//	{
	//		mName = CMonoHelpers::GetString(g_Domain, name);
	//	}
	//	mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_name, mName);
	//	mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_frag, &pfrag);
	//	mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_death, &pdeath);
	//	if(mAddress)
	//	{
	//		mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_ip, mAddress);
	//	}
	//	mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_language, lng);
	//	mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_avgLatency, &avgLatency);
	//	mono_field_set_value(this->m_players[index].MPlayer, this->m_Field_ClsPlayer_timeConnected, &timeConnected);
	//}
}
