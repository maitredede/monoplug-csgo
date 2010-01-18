/* ======== SimpleStats ========
* Copyright (C) 2004-2008 Erling K. Sæterdal
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): Erling K. Sæterdal ( EKS )
* Credits:
* Helping on misc errors/functions: BAILOPAN,sslice,devicenull,PMOnoTo,cybermind ( most who idle in #sourcemod on GameSurge realy )
* ============================ */

#include "recipientfilters.h"
#include "utlvector.h"
#include "MiniStatsCore.h"

extern stPlayerInfo g_UserInfo[MAXPLAYERS+1];

RecipientFilter::RecipientFilter()
{
	m_InitMessage = false;
	m_Reliable = false;
}

RecipientFilter::~RecipientFilter()
{
}

void RecipientFilter::MakeReliable()
{
	m_Reliable = true;
}

void RecipientFilter::MsgRecipients()
{
	Msg("Displaying recipients:\n");
	for (int i=0; i<GetRecipientCount(); i++)
	{
		Msg("Recipient #%d: %d\n", i, GetRecipientIndex(i));
	}
}

int RecipientFilter::GetRecipientCount() const
{
	return m_Recipients.Size();
}

int RecipientFilter::GetRecipientIndex(int slot) const
{
	if (slot < 0 || slot >= GetRecipientCount())
		return -1;

	return m_Recipients[slot];
}

bool RecipientFilter::IsInitMessage() const
{
	return m_InitMessage;
}

bool RecipientFilter::IsReliable() const
{
	return m_Reliable;
}

void RecipientFilter::AddAllPlayers(int maxClients)
{
	for (int i=1; i<=maxClients; i++)
	{
		if(PluginCore.GetEngineUtils()->FastIsUserConnected(i) == true && !g_UserInfo[i].IsBot && g_UserInfo[i].ShowStats)
			m_Recipients.AddToTail(i);
	}
}

/*
void RecipientFilter::AddPlayer(edict_t *e)
{
	int i = g_MSCore.m_Engine->IndexOfEdict(e);
	if(g_IsConnected[i] == true)
		m_Recipients.AddToTail(g_MSCore.m_Engine->IndexOfEdict(e));
}
*/

void RecipientFilter::AddPlayer(int index)
{
	if(PluginCore.GetEngineUtils()->FastIsUserConnected(index) == true)
		m_Recipients.AddToTail(index);
}
