#ifndef _INCLUDE_EventListeners_CLASS
#define _INCLUDE_EventListeners_CLASS

#include "MiniStatsCore.h"

class EventListenPlayerDeath :  public IGameEventListener2
{
public:
	void FireGameEvent(IGameEvent *event); 
};
class EventListenPlayerHurt :  public IGameEventListener2
{
public:
	void FireGameEvent(IGameEvent *event); 
};
class EventListenPlayerWeaponFire :  public IGameEventListener2
{
public:
	void FireGameEvent(IGameEvent *event); 
};

#endif //_INCLUDE_EventListeners_CLASS