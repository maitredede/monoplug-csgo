#ifndef _DOTNETPLUG_EVENT_LISTENER_H_
#define _DOTNETPLUG_EVENT_LISTENER_H_
#ifdef _WIN32
#pragma once
#endif

#include <igameevents.h>
#include "GameEvent.h"

class EventListener :public IGameEventListener2
{
public:
	EventListener(GameEvent ge);
	~EventListener();
public:
	void FireGameEvent(IGameEvent *event);
	int	 GetEventDebugID(void);
private:
	GameEvent m_ge;
};

#endif //_DOTNETPLUG_EVENT_LISTENER_H_