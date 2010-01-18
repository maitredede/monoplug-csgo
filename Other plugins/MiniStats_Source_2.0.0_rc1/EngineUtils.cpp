#include "EngineUtils.h"
#include "MiniStatsCore.h"

const char* EngineUtils::GetTeamName(int id)
{
	int TeamIndex = PluginCore.GetEngineUtils()->GetUserTeam(id);

	if(g_ModSettings.GameMod == MOD_CSTRIKE)	
	{
		switch (TeamIndex)
		{
		case 1: return "Spectator";
		case 2: return "Terrorist";
		case 3: return "Counter-Terrorist";
		}
	}
	else if(g_ModSettings.GameMod == MOD_DOD)	
	{
		switch (TeamIndex)
		{
		case 1: return "Spectator";
		case 2: return "Allied";
		case 3: return "Axis";
		}
	}
	else if(g_ModSettings.GameMod == MOD_EMPIRES)	
	{
		switch (TeamIndex)
		{
		case 1: return "Spectator";
		case 2: return "Northern Faction";
		case 3: return "Brenodi Empire";
		}
	}
	else if(g_ModSettings.GameMod == MOD_PVKII)	
	{
		switch (TeamIndex)
		{
		case 1: return "Spectator";
		case 2: return "Pirates";
		case 3: return "Vikings";
		case 4: return "Knights";
		}
	}
	else if(g_ModSettings.GameMod == MOD_DYSTOPIA)	
	{
		switch (TeamIndex)
		{
		case 1: return "Spectator";
		case 2: return "Punks";
		case 3: return "Cops";
		}
	}
	else if(g_ModSettings.GameMod == MOD_INSURGENCY)	
	{
		switch (TeamIndex)
		{
		case 1: return "Marines";
		case 2: return "Iraq";
		case 3: return "Spectator";
		}
	}
	else if(g_ModSettings.GameMod == MOD_TF2)	
	{
		switch (TeamIndex)
		{		
		case 4: return "Red";
		case 3: return "Blue";
		case 2: return "Spectator";
		case 1: return "Random";
		}
	}
	else
	{
		switch (TeamIndex)
		{
		case 1: 
			return "Spectator";
		case 2: 
			return PluginCore.GetHLSVar().GetTeam1Name();
		case 3: 
			return PluginCore.GetHLSVar().GetTeam2Name();
		}
	}
	return "Error";
}


int EngineUtils::GetUserTeam(int id)
{
	IPlayerInfo *pInfo = m_InfoMngr->GetPlayerInfo(g_UserInfo[id].PlayerEdict);
	if(!pInfo || g_IsConnected[id] == false)
		return 0;

	return pInfo->GetTeamIndex();
}

int EngineUtils::FindPlayer(int UserID)	// Finds the player index based on userid.
{
	for(int i=1;i<=MaxClients;i++)
	{
		if(g_IsConnected[i] == true)
		{
			if(UserID == g_UserInfo[i].Userid )	// Name or steamid matches TargetInfo so we return the index of the player
			{
				return i;
			}
		}
	}
	return -1;
}

int EngineUtils::FindPlayer(const char *Name)	// Finds the player index based on name.
{
	for(int i=1;i<=MaxClients;i++)
	{
		if(g_IsConnected[i] == true)
		{
			const char *PlayerName = GetPlayerName(i);
			if(strcmp(PlayerName,Name) == 0)	
			{
				return i;
			}
		}
	}
	return -1;
}

const char *EngineUtils::GetPlayerWeapon(int id)
{
	IPlayerInfo *pInfo = PluginCore.PlayerInfo()->GetPlayerInfo(g_UserInfo[id].PlayerEdict);
	if(!pInfo)
		return NULL;

	const char *pWeapon = pInfo->GetWeaponName();
	if(!pWeapon)
		return NULL;

	return pWeapon;
}

const char *EngineUtils::GetPlayerWeaponNamePretty(int id)
{
	if(id == -1)
		return NULL;
	IPlayerInfo *pInfo = PluginCore.PlayerInfo()->GetPlayerInfo(g_UserInfo[id].PlayerEdict);
	if(!pInfo)
		return NULL;

	const char *pWeapon = pInfo->GetWeaponName();
	if(!pWeapon)
		return NULL;

	if(strlen(pWeapon) > 7)
		pWeapon = &(pWeapon[7]);

	return pWeapon;
}

const Vector EngineUtils::GetPlayerOrigin(int id)
{
	if(id == -1)
	{
		Vector BadReturn;
		BadReturn.x = 0;
		BadReturn.y = 0;
		BadReturn.z = 0;
			
		return BadReturn;
	}
	IPlayerInfo *pInfo = PluginCore.PlayerInfo()->GetPlayerInfo(g_UserInfo[id].PlayerEdict);
	if(!pInfo)
	{
		Vector BadReturn;
		BadReturn.x = 0;
		BadReturn.y = 0;
		BadReturn.z = 0;

		return BadReturn;
	}

	return pInfo->GetAbsOrigin();
}

const char *EngineUtils::GetPlayerName(int id)
{
	return m_Engine->GetClientConVarValue(id,"name");
}

void EngineUtils::SendCSay(int id,const char *msg, ...) 
{
	char vafmt[192];
	va_list ap;
	va_start(ap, msg);
	_vsnprintf(vafmt,191,msg,ap);
	va_end(ap);

	for(int i=1;i<=MaxClients;i++)	
	{
		if(g_IsConnected[i] && g_UserInfo[i].IsBot == false)
			m_Engine->ClientPrintf(g_UserInfo[i].PlayerEdict,vafmt);
	}

	if(g_ModSettings.ValveCenterSay)
	{
		RecipientFilter mrf;

		if(id == 0)
			mrf.AddAllPlayers(PluginCore.GetEngineUtils()->MaxClients);
		else
			mrf.AddPlayer(id);

		bf_write *buffer = m_Engine->UserMessageBegin(static_cast<IRecipientFilter *>(&mrf), g_ModSettings.TextMsg);
		buffer->WriteByte(HUD_PRINTCENTER);
		buffer->WriteString(msg);
		m_Engine->MessageEnd();
	}
	else
		MessagePlayer(id,vafmt);
}

void EngineUtils::LogPrint( const char *msg, ...)
{
	char vafmt[512];
	va_list ap;
	va_start(ap, msg);
	int len = _vsnprintf(vafmt, 511, msg, ap);
	va_end(ap);
	len += _snprintf(&(vafmt[len]),511-len," \n");

	m_Engine->LogPrint(vafmt);
}

void EngineUtils::FakeClientSay(int id, const char *msg, ...)
{
	// What we fake
	//L 12/28/2006 - 21:17:16: "EKS<2><STEAM_ID_LAN><CT>" say "say OMG"

	char vafmt[192];
	va_list ap;
	va_start(ap, msg);
	int len = _vsnprintf(vafmt, 189, msg, ap);
	va_end(ap);

	char FakeSay[192];
	_snprintf(FakeSay,191,"\"%s<%d><%s><%s>\" say \"%s\"\n",PluginCore.GetEngineUtils()->GetPlayerName(id),g_UserInfo[id].Userid,g_UserInfo[id].Steamid,PluginCore.GetEngineUtils()->GetTeamName(id),vafmt);

	m_Engine->LogPrint(FakeSay);
}

void EngineUtils::ServerCommand(const char *fmt, ...)
{
	char buffer[512];
	va_list ap;
	va_start(ap, fmt);
	_vsnprintf(buffer, sizeof(buffer)-2, fmt, ap);
	va_end(ap);
	strcat(buffer,"\n");
	m_Engine->ServerCommand(buffer);
}

void EngineUtils::MessagePlayer(int index, const char *msg, ...)
{
	RecipientFilter rf;
	if (index > MaxClients || index < 0)
		return;

	if (index == 0)
	{
		rf.AddAllPlayers(MaxClients);
	} else {
		rf.AddPlayer(index);
	}
	char vafmt[240];
	va_list ap;
	va_start(ap, msg);
	int len = _vsnprintf(vafmt, 512, msg, ap);
	va_end(ap);
	len += _snprintf(&(vafmt[len]),511-len," \n");

	char *ptr = vafmt;
	len = strlen(vafmt);
	char save = 0;

	while (true)
	{
		if (len > MaxPrSend)
		{
			save = ptr[MaxPrSend];
			ptr[MaxPrSend] = '\0';
		}
		bf_write *buffer = m_Engine->UserMessageBegin(static_cast<IRecipientFilter *>(&rf), g_ModSettings.SayText);
		
		buffer->WriteByte( (len > MaxPrSend) ? 1 : 0 );
		buffer->WriteString(ptr);
		buffer->WriteByte(0);
		PluginCore.GetEngine()->MessageEnd();
		if (len > MaxPrSend)
		{
			ptr[MaxPrSend] = save;
			ptr = &(ptr[MaxPrSend]);
			len -= MaxPrSend;
		} else {
			break;
		}
	}
}
void EngineUtils::MessagePlayer2(int index, const char *msg, ...) // Sends message and colors the nick in team color. Probably only works in valve mods
{
	RecipientFilter rf;
	if (index > MaxClients || index < 0)
		return;

	if (index == 0)
	{
		rf.AddAllPlayers(MaxClients);
	} else {
		rf.AddPlayer(index);
	}
	char vafmt[240];
	va_list ap;
	va_start(ap, msg);
	int len = _vsnprintf(vafmt, 239, msg, ap);
	va_end(ap);
	len += _snprintf(&(vafmt[len]),239-len," \n");

	bf_write *buffer = m_Engine->UserMessageBegin(static_cast<IRecipientFilter *>(&rf), g_ModSettings.SayText2);
	buffer->WriteByte(1);
	buffer->WriteByte(0);
	buffer->WriteString(vafmt);
	m_Engine->MessageEnd();
}

void EngineUtils::SendHintMsg(int id,char *text)
{
	RecipientFilter mrf;
	char FormatedText[192];
	if(id == 0)
		mrf.AddAllPlayers(MaxClients);
	else
		mrf.AddPlayer(id);

	_snprintf(FormatedText,191,"%s",text);
	int len = strlen(FormatedText);
	if(len >= 191)
		return; // that would be a buffer overrun

	if(len > 30) // Since the client dont automatically add \n when needed, we have to do it.
	{
		int LastAdded=0;
		for(int i=0;i<len;i++)
		{
			int t = GetNextSpaceCount(text,i,len);
			char b = FormatedText[i];

			if(FormatedText[i] == ' ' && LastAdded > 30 && (len-i) > 10 || (GetNextSpaceCount(text,i+1,len) + LastAdded)  > 34)
			{
				FormatedText[i] = '\n';
				LastAdded = 0;
			}
			else
				LastAdded++;
		}
	}

	bf_write *buffer = m_Engine->UserMessageBegin(static_cast<IRecipientFilter *>(&mrf), g_ModSettings.HintText);
	buffer->WriteByte(-1);
	buffer->WriteString(FormatedText);
	m_Engine->MessageEnd();	
}

void EngineUtils::SendHintMsgNoFormat(int id,char *text)
{
	RecipientFilter mrf;
	if(id == 0)
		mrf.AddAllPlayers(MaxClients);
	else
		mrf.AddPlayer(id);

	bf_write *buffer = m_Engine->UserMessageBegin(static_cast<IRecipientFilter *>(&mrf), g_ModSettings.HintText);
	buffer->WriteByte(-1);
	buffer->WriteString(text);
	m_Engine->MessageEnd();	
}
// Private functions
int EngineUtils::GetNextSpaceCount(const char *Text,int CurIndex,int Maxlen)
{
	int Count=0;
	for(int i=CurIndex;i<Maxlen;i++)
	{
		if(Text[i] == ' ')
			return Count;
		else
			Count++;
	}

	return Count;
}