#ifndef _DOTNETPLUG_EVENT_MANAGER_H_
#define _DOTNETPLUG_EVENT_MANAGER_H_
#ifdef _WIN32
#pragma once
#endif

#include <igameevents.h>
#include "GameEvent.h"

class EventManager :public IGameEventListener2
{
public:
	EventManager();
	~EventManager();
	void AttachEvents();
	void DetachEvents();
public:
	void FireGameEvent(IGameEvent *event);
	int	 GetEventDebugID(void);

private:
	void AddHandler(GameEvent e);
};

extern EventManager g_EventManager;

#endif //_DOTNETPLUG_EVENT_MANAGER_H_