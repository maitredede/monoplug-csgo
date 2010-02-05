#include "CMonoPlugin.h"
#include "monoCallbacks.h"
#include "tools.h"
#include <inetchannelinfo.h>

namespace MonoPlugin
{
	bool CMonoPlugin::InitMono(const char* dllFile, const char* rootDir, char *error, size_t maxlen)
	{
		//Init the runtime
		if(!g_Domain)
		{
			if(!CMonoHelpers::GetDomain(dllFile, rootDir, g_Domain, error, maxlen)) return false;
		}

		//Load the assembly
		if(!CMonoHelpers::GetAssembly(dllFile, g_Domain, this->m_assembly, error, maxlen)) return false;
		if(!CMonoHelpers::GetImage(dllFile, this->m_assembly, this->m_image, error, maxlen)) return false;

		//Get references to types, functions and members
		if(!CMonoHelpers::GetClass(this->m_image, "MonoPlug", "ClsMain", this->m_class_ClsMain, error, maxlen)) return false;
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Init()", this->m_ClsMain_Init, error, maxlen)) return false;
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Shutdown()", this->m_ClsMain_Shutdown, error, maxlen)) return false;
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:GameFrame()", this->m_ClsMain_GameFrame, error, maxlen)) return false;
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:AllPluginsLoaded()", this->m_ClsMain_AllPluginsLoaded, error, maxlen)) return false;

		if(!CMonoHelpers::GetClass(this->m_image, "MonoPlug", "ClsPlayer", this->m_Class_ClsPlayer, error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_id, "_id", error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_name, "_name", error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_frag, "_frag", error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_death, "_death", error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_ip, "_ip", error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_language, "_language", error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_avgLatency, "_avgLatency", error, maxlen)) return false;
		if(!CMonoHelpers::GetField(this->m_Class_ClsPlayer, this->m_Field_ClsPlayer_timeConnected, "_timeConnected", error, maxlen)) return false;

		//Add internal calls
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Msg", (const void*)Mono_Msg);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Log", (const void*)Mono_Log);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_DevMsg", (const void*)Mono_DevMsg);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Warning", (const void*)Mono_Warning);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Error", (const void*)Mono_Error);

		//Console printer
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_AttachConsole", (const void*)Mono_AttachConsole);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_DetachConsole", (const void*)Mono_DetachConsole);
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Raise_ConMessage(bool,bool,int,int,int,int,string)", this->m_ClsMain_ConPrint, error, maxlen)) return false;

		//ConCommands
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_RegisterConCommand", (const void*)Mono_RegisterConCommand);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_UnregisterConCommand", (const void*)Mono_UnregisterConCommand);
		
		//ConVars
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_RegisterConVar", (const void*)Mono_RegisterConVar);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_UnregisterConVar", (const void*)Mono_UnregisterConVar);
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Raise_ConVarChange(ulong)", this->m_ClsMain_Raise_ConVarChange, error, maxlen)) return false;

		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_GetString", (const void*)Mono_Convar_GetString);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_SetString", (const void*)Mono_Convar_SetString);

		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_GetBoolean", (const void*)Mono_Convar_GetBoolean);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_Convar_SetBoolean", (const void*)Mono_Convar_SetBoolean);

		//LevelShutdown
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventAttach_LevelShutdown", (const void*)Mono_EventAttach_LevelShutdown);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventDetach_LevelShutdown", (const void*)Mono_EventDetach_LevelShutdown);
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Raise_LevelShutdown()", this->m_ClsMain_Raise_LevelShutdown, error, maxlen)) return false;

		//ServerActivate
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventAttach_ServerActivate", (const void*)Mono_EventAttach_ServerActivate);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventDetach_ServerActivate", (const void*)Mono_EventDetach_ServerActivate);
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Raise_ServerActivate(int)", this->m_ClsMain_Raise_ServerActivate, error, maxlen)) return false;



		//ClientDisconnect
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventAttach_ClientDisconnect", (const void*)Mono_EventAttach_ClientDisconnect);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventDetach_ClientDisconnect", (const void*)Mono_EventDetach_ClientDisconnect);
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Raise_ClientDisconnect(MonoPlug.ClsPlayer)", this->m_ClsMain_Raise_ClientDisconnect, error, maxlen)) return false;
		//ClientPutInServer
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventAttach_ClientPutInServer", (const void*)Mono_EventAttach_ClientPutInServer);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_EventDetach_ClientPutInServer", (const void*)Mono_EventDetach_ClientPutInServer);
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Raise_ClientPutInServer(MonoPlug.ClsPlayer)", this->m_ClsMain_Raise_ClientPutInServer, error, maxlen)) return false;

		//Various
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_ClientDialogMessage", (const void*)Mono_ClientDialogMessage);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_ServerCommand", (const void*)Mono_ServerCommand);
		mono_add_internal_call ("MonoPlug.NativeMethods::Mono_ClientMessage", (const void*)Mono_ClientMessage);

		//IGameEventListener2 events
		MP_EVENT_INIT(server_spawn, this->m_event_server_spawn,
			"MonoPlug.ClsMain:Event_server_spawn(string,string,string,string,string,int,string,bool,bool)", this->m_ClsMain_event_server_spawn,
			"MonoPlug.NativeMethods::Mono_EventAttach_server_spawn",
			"MonoPlug.NativeMethods::Mono_EventDetach_server_spawn");
		MP_EVENT_INIT(server_shutdown, this->m_event_server_shutdown,
			"MonoPlug.ClsMain:Event_server_shutdown(string)", this->m_ClsMain_event_server_shutdown,
			"MonoPlug.NativeMethods::Mono_EventAttach_server_shutdown",
			"MonoPlug.NativeMethods::Mono_EventDetach_server_shutdown");
		MP_EVENT_INIT(player_connect, this->m_event_player_connect,
			"MonoPlug.ClsMain:Event_player_connect(string,int,int,string,string,MonoPlug.ClsPlayer)", this->m_ClsMain_event_player_connect,
			"MonoPlug.NativeMethods::Mono_EventAttach_player_connect",
			"MonoPlug.NativeMethods::Mono_EventDetach_player_connect");

		MP_EVENT_INIT(player_disconnect, this->m_event_player_disconnect,
			"MonoPlug.ClsMain:Event_player_disconnect(int,string,string,string,MonoPlug.ClsPlayer)", this->m_ClsMain_event_player_disconnect,
			"MonoPlug.NativeMethods::Mono_EventAttach_player_disconnect",
			"MonoPlug.NativeMethods::Mono_EventDetach_player_disconnect");

		//Create main instance
		this->m_main = CMonoHelpers::ClassNew(g_Domain, this->m_class_ClsMain);

		return true;
	}

	bool CMonoPlugin::StartMono(char *error, size_t maxlen)
	{
		MonoObject* ret = CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Init, NULL);
		if(ret)
		{
			bool bRet = *(bool*)mono_object_unbox(ret);
			return bRet;
		}
		else
		{
			return false;
		}
	}

	MonoObject* CMonoPlugin::GetPlayer(int userid)
	{
		edict_t* pEntity = EdictOfUserId(userid);
		if(pEntity && !pEntity->IsFree())
		{
			return this->GetPlayer(pEntity);
		}
		return NULL;
	}

	MonoObject* CMonoPlugin::GetPlayer(edict_t* pEntity)
	{
		MonoObject* player = CMonoHelpers::ClassNew(g_Domain, this->m_Class_ClsPlayer);

		//Fill player data
		int playerId = g_engine->GetPlayerUserId(pEntity);

		IPlayerInfo* pi = g_PlayerInfoManager->GetPlayerInfo(pEntity);
		INetChannelInfo* net = g_engine->GetPlayerNetInfo(playerId);
		float avgLatency = -1.0;
		float timeConnected = -1.0;
		MonoString* address;
		if(net)
		{
			avgLatency = net->GetAvgLatency(MAX_FLOWS);
			timeConnected = net->GetTimeConnected();
			address = CMonoHelpers::GetString(g_Domain, net->GetAddress());
		}
		else
		{
			address = CMonoHelpers::GetString(g_Domain, NULL);
		}
		int pfrag = pi->GetFragCount();
		int pdeath = pi->GetDeathCount();

		mono_field_set_value(player, this->m_Field_ClsPlayer_id, &playerId);
		mono_field_set_value(player, this->m_Field_ClsPlayer_name, CMonoHelpers::GetString(g_Domain, pi->GetName()));
		mono_field_set_value(player, this->m_Field_ClsPlayer_frag, &pfrag);
		mono_field_set_value(player, this->m_Field_ClsPlayer_death, &pdeath);
		mono_field_set_value(player, this->m_Field_ClsPlayer_ip, address);
		mono_field_set_value(player, this->m_Field_ClsPlayer_language, CMonoHelpers::GetString(g_Domain, g_engine->GetClientConVarValue(playerId, "cl_language")));
		mono_field_set_value(player, this->m_Field_ClsPlayer_avgLatency, &avgLatency);
		mono_field_set_value(player, this->m_Field_ClsPlayer_timeConnected, &timeConnected);

		return player;
	}
}
