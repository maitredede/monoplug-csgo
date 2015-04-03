#include "EventListener.h"
#include "Managed.h"

EventListener::EventListener(GameEvent ge)
{
	this->m_ge = ge;
}

EventListener::~EventListener()
{
}

void EventListener::FireGameEvent(IGameEvent *event)
{
	g_Managed.RaiseGameEvent(this->m_ge, event);
}

int	 EventListener::GetEventDebugID(void)
{
	return EVENT_DEBUG_ID_INIT;
}