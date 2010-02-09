#ifndef _EVENTS_H_
#define _EVENTS_H_

#include "monoBase.h"
#include "pluginterfaces.h"

namespace MonoPlugin
{
#define MP_EVENT_DO_ATTACH(eventName, eventPtr, methodPtr) \
	if(!eventPtr) \
	{ \
		eventPtr = new CEvent_##eventName(methodPtr); \
		g_GameEventManager->AddListener(eventPtr, #eventName, true); \
	};

#define MP_EVENT_DO_DETACH(eventName, eventPtr, methodPtr) \
	if(eventPtr) \
	{ \
		g_GameEventManager->RemoveListener(eventPtr); \
		delete eventPtr; \
		eventPtr = NULL; \
	};

/////
// DECL
/////
#define MP_EVENT_DECL(eventName) class CEvent_##eventName: public IGameEventListener2 \
	{ \
	private: \
		MonoMethod* m_method; \
	public: \
		CEvent_##eventName(MonoMethod* method) { this->m_method = method; } \
		void FireGameEvent(IGameEvent *evt);  \
	}; \
	void Mono_EngineEvent_Attach_##eventName(); \
	void Mono_EngineEvent_Detach_##eventName(); \

/////
// CODE
/////
#define MP_EVENT_CODE(eventName, eventPtr, methodPtr) \
	void Mono_EngineEvent_Attach_##eventName() \
	{ \
		MP_EVENT_DO_ATTACH(eventName, eventPtr, methodPtr); \
	} \
	void Mono_EngineEvent_Detach_##eventName() \
	{ \
		MP_EVENT_DO_DETACH(eventName, eventPtr, methodPtr); \
	} \
	void CEvent_##eventName::FireGameEvent(IGameEvent *evt)


/////
// INIT
/////
#define MP_EVENT_INIT(eventName, eventPtr, methodSig, methodPtr, methodSigAttach, methodSigDetach) \
	eventPtr = NULL; \
	if(!CMonoHelpers::GetMethod(this->m_image, methodSig, methodPtr, error, maxlen)) return false; \
	mono_add_internal_call(methodSigAttach, (void*)Mono_EngineEvent_Attach_##eventName); \
	mono_add_internal_call(methodSigDetach, (void*)Mono_EngineEvent_Detach_##eventName);

	//http://wiki.alliedmods.net/Generic_Source_Server_Events

	MP_EVENT_DECL(server_spawn);
	MP_EVENT_DECL(server_shutdown);
	//MP_EVENT_DECL(server_addban);
	//MP_EVENT_DECL(server_removeban);
	MP_EVENT_DECL(player_connect);
	//MP_EVENT_DECL(player_info);
	MP_EVENT_DECL(player_disconnect);
	//MP_EVENT_DECL(player_activate);
	//MP_EVENT_DECL(player_say);
}

#endif //_EVENTS_H_

