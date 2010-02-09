#include "CMonoPlayer.h"
#include <inetchannelinfo.h>
#include "CMonoHelpers.h"
#include "CMonoPlugin.h"

namespace MonoPlugin
{
	CMonoPlayer::CMonoPlayer(int userId, bool isBot)
	{
		this->m_userId = userId;
		this->m_isBot = isBot;
		this->m_obj = CMonoHelpers::ClassNew(g_Domain, g_MonoPlugin.m_Class_ClsPlayer);

		mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_userId, &this->m_userId);
		mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_isBot, &this->m_isBot);

	}

	MonoObject* CMonoPlayer::GetPlayer()
	{
		return this->m_obj;
	}

	CMonoPlayer::~CMonoPlayer()
	{
		if(this->m_obj)
		{
			delete this->m_obj;
			this->m_obj = NULL;
		}
	}

	bool CMonoPlayer::IsBot()
	{
		return this->m_isBot;
	}

	int CMonoPlayer::UserId()
	{
		return this->m_userId;
	}

	void CMonoPlayer::SetConnected(bool connecting, bool connected)
	{
		mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_isConnecting, &connecting);
		mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_isConnected, &connected);
	}

	void CMonoPlayer::SetData(const char* name, const char* address)
	{
		sprintf(this->m_name, "%s", name);

		mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_name, CMonoHelpers::GetString(g_Domain, name));
		if(address)
		{
			mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_ip, CMonoHelpers::GetString(g_Domain, address));
		}
	}

	void CMonoPlayer::SetSteam(bool valid, const char* networkid)
	{
		mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_steamId, CMonoHelpers::GetString(g_Domain, networkid));
		mono_field_set_value(this->m_obj, g_MonoPlugin.m_Field_ClsPlayer_isSteamValid, &valid);
	}

	const char* CMonoPlayer::Name()
	{
		return this->m_name;
	}

	//CMonoPlayer::CMonoPlayer(CMonoPlugin* plug, int userid, int index, bool bot, edict_t* pEntity)
	//{
	//	this->m_plugin = plug;

	//	this->m_userid = userid;
	//	this->m_index = index;
	//	this->m_bot = bot;
	//	this->m_entity = pEntity;

	//	this->m_obj = CMonoHelpers::ClassNew(g_Domain, this->m_plugin->m_Class_ClsPlayer);
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_id, &this->m_userid);
	//}

	//bool CMonoPlayer::IsBot()
	//{
	//	return this->m_bot;
	//}

	//int CMonoPlayer::UserId()
	//{
	//	return this->m_userid;
	//}

	//edict_t* CMonoPlayer::Entity()
	//{
	//	return this->m_entity;
	//}
	//
	//MonoObject* CMonoPlayer::PlayerObject()
	//{
	//	return this->m_obj;
	//}

	//void CMonoPlayer::SetData(const char* name, const char* address, const char* networkid)
	//{
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_name, CMonoHelpers::GetString(g_Domain, name));
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_ip, CMonoHelpers::GetString(g_Domain, address));
	//	//mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_, CMonoHelpers::GetString(g_Domain, pi->GetName()));
	//}

	//void CMonoPlayer::RefreshData()
	//{
	//	IPlayerInfo* pi = g_PlayerInfoManager->GetPlayerInfo(this->m_entity);
	//	INetChannelInfo* net = g_engine->GetPlayerNetInfo(this->m_userid);

	//	float avgLatency = -1.0;
	//	float timeConnected = -1.0;
	//	MonoString* address;

	//	if(net)
	//	{
	//		avgLatency = net->GetAvgLatency(MAX_FLOWS);
	//		timeConnected = net->GetTimeConnected();
	//		address = CMonoHelpers::GetString(g_Domain, net->GetAddress());
	//	}
	//	else
	//	{
	//		address = CMonoHelpers::GetString(g_Domain, NULL);
	//	}
	//	int pfrag = -1;
	//	int pdeath = -1;
	//	if(pi)
	//	{
	//		pfrag = pi->GetFragCount();
	//		pdeath = pi->GetDeathCount();
	//	}

	//	MonoString* lng;
	//	if(this->m_bot)
	//	{
	//		lng = CMonoHelpers::GetString(g_Domain, NULL);
	//	}
	//	else
	//	{
	//		lng = CMonoHelpers::GetString(g_Domain, g_engine->GetClientConVarValue(this->m_index, "cl_language"));
	//	}

	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_name, CMonoHelpers::GetString(g_Domain, pi->GetName()));
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_frag, &pfrag);
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_death, &pdeath);
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_ip, address);
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_language, lng);
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_avgLatency, &avgLatency);
	//	mono_field_set_value(this->m_obj, this->m_plugin->m_Field_ClsPlayer_timeConnected, &timeConnected);
	//}
}
