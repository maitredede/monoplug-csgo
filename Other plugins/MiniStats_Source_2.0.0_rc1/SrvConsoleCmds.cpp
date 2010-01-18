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

#include "MiniStatsCore.h"
#include "BATMenu.h"
#include "SrvConsoleCmds.h"
#include "recipientfilters.h"
#include <string.h>
#include <bitbuf.h>
#include <ctype.h>

extern HLStatsIngame g_MSCore;
int g_LastPlayerIndex = -1;	// We store the index of the last nick in ms_psay2 / ms_psay msg


ConCommand HLSSrvCmd1( "ms_psay", SrvConsoleCmds::CmdPSayViaChat, "ms_psay <userid>" );
ConCommand HLSSrvCmd2( "ms_psay2", SrvConsoleCmds::CmdPSay2ViaChat, "ms_psay2 <userid>" );
ConCommand HLSSrvCmd3( "ms_hint", SrvConsoleCmds::CmdHintMsg, "ms_hint <userid>");
ConCommand HLSSrvCmd4( "ms_csay", SrvConsoleCmds::CmdCenterSay, "ms_csay <name>");
ConCommand HLSSrvCmd5( "ms_browse", SrvConsoleCmds::CmdShowMOTD, "ms_browse <userid> " );
ConCommand HLSSrvCmd6( "ms_msay", SrvConsoleCmds::CmdMenuSay, "ms_msay <userid>");
ConCommand HLSSrvCmd7( "ms_swap", SrvConsoleCmds::CmdSwapUserTeam, "ms_swap <userid>");
/*
RegServerCmd("hlx_sm_redirect",      hlx_sm_redirect);
RegServerCmd("hlx_sm_tsay",          hlx_sm_tsay);
RegServerCmd("hlx_sm_player_action", hlx_sm_player_action);
RegServerCmd("hlx_sm_team_action",   hlx_sm_team_action);
RegServerCmd("hlx_sm_world_action",  hlx_sm_world_action);
*/

#if ORANGEBOX_ENGINE == 1
void SrvConsoleCmds::CmdShowMOTD(const CCommand &args)
{
	g_LastCCommand = args;	
#else
void SrvConsoleCmds::CmdShowMOTD()
{
#endif
	// "ms_browse "9" http://steamfriends.hlstatsx.com/ingame.php?game=css&mode=targets&player=33"
	//ms_browse "9" http://steamfriends.hlstatsx.com/ingame.php?game=css&mode=targets&player=33"

	PreSamplerStart("SrvConsoleCmds::CmdShowMOTD")
	int id = g_MSCore.GetEngineUtils()->FindPlayer(atoi(g_LastCCommand.Arg(1)));

	const char * pEntireLine = g_LastCCommand.ArgS();
	int StartIndex = -1;
	int StrLen = strlen(pEntireLine);

	for (int i=0;i<StrLen;i++)
	{
		if(pEntireLine[i] == 'h' || pEntireLine[i] == 'H' )
		{
			StartIndex = i;
			break;
		}
	}

	if(id == -1 || StartIndex == -1)
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send MOTD box but did not contain a message");
		PreSamplerEnd("SrvConsoleCmds::CmdShowMOTD")
		return;
	}

	char Buffer[240];
	_snprintf(Buffer,239,"%s",pEntireLine + StartIndex);
	g_MSCore.StrRemoveQuotes(Buffer);

	g_MSCore.GetEngineUtils()->LogPrint("Debug MS: ma_browse url: \"%s\"",Buffer);
	ShowMOTD(id,Buffer);
	PreSamplerEnd("SrvConsoleCmds::CmdShowMOTD")
}


#if ORANGEBOX_ENGINE == 1
void SrvConsoleCmds::CmdCenterSay(const CCommand &args)
{
	g_LastCCommand = args;
#else
void SrvConsoleCmds::CmdCenterSay()
{
#endif
	PreSamplerStart("SrvConsoleCmds::CmdCenterSay")
	char CmdCenterSay[192];

	_snprintf(CmdCenterSay,191,"%s",g_LastCCommand.ArgS());
	g_MSCore.GetEngineUtils()->SendCSay(0,CmdCenterSay);
	PreSamplerEnd("SrvConsoleCmds::CmdCenterSay")
}


#if ORANGEBOX_ENGINE == 1
void SrvConsoleCmds::CmdHintMsg(const CCommand &args)
{
	g_LastCCommand = args;
#else
void SrvConsoleCmds::CmdHintMsg()
{
#endif	
	//ms_hint "3" Welcome -[SF]-CaNaBz!"
	PreSamplerStart("SrvConsoleCmds::CmdHintMsg")

	int ArgCount = g_LastCCommand.ArgC();
	int len=0;
	int id = g_MSCore.GetEngineUtils()->FindPlayer(atoi(g_LastCCommand.Arg(1)));
	char Buffer[240];

	if(id == -1 )
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send private message but did not send a valid userid");
		PreSamplerEnd("SrvConsoleCmds::CmdHintMsg")
		return;
	}

	if(ArgCount >= 3)
	{
		len += _snprintf(&(Buffer[len]),239-len,"%s",g_LastCCommand.Arg(2));

		for(int i=3;i<ArgCount;i++)
		{
			len += _snprintf(&(Buffer[len]),239-len," %s",g_LastCCommand.Arg(i));
		}
	}
	else
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send hint message but did not contain a message");
		PreSamplerEnd("SrvConsoleCmds::CmdHintMsg")
		return;
	}

	g_MSCore.GetEngineUtils()->SendHintMsg(id,Buffer);
	PreSamplerEnd("SrvConsoleCmds::CmdHintMsg")
}

#if ORANGEBOX_ENGINE == 1
void SrvConsoleCmds::CmdPSayViaChat(const CCommand &args)
{
	g_LastCCommand = args;
#else
void SrvConsoleCmds::CmdPSayViaChat()
{
#endif
	// Tobi should shot for doing this. every time someone sends place command the remote server a list of ms_psay for every player
	//ms_psay "10" -[SF]- Viper is on rank 169 of 143,407 with 6,495 points;
	//ms_psay "11" -[SF]- Viper is on rank 169 of 143,407 with 6,495 points;
	//ms_psay "8" -[SF]- Viper is on CmdPSayViaChat 169 of 143,407 with 6,495 points;"
	PreSamplerStart("SrvConsoleCmds::CmdPSayViaChat")
	char Buffer[240];

	int ArgCount = g_LastCCommand.ArgC();
	int len=0;
	int id = g_MSCore.GetEngineUtils()->FindPlayer(atoi(g_LastCCommand.Arg(1)));

	if(id == -1 || id == 0)
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send private message but did not pass a valid userid");
		PreSamplerEnd("SrvConsoleCmds::CmdPSayViaChat")
		return;
	}

	if(ArgCount >= 2)
	{
		len += _snprintf(&(Buffer[len]),239-len,"%s",g_LastCCommand.Arg(2));

		for(int i=3;i<ArgCount;i++)
		{
			len += _snprintf(&(Buffer[len]),239-len," %s",g_LastCCommand.Arg(i));
		}
	}
	else
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send private message but did not contain a message");
		PreSamplerEnd("SrvConsoleCmds::CmdPSayViaChat")
		return;
	}

	//Now we need to find whats the player index is, from his nick name
	int PlayerNameIndex = -1;

	if(g_LastPlayerIndex != -1 && strstr(Buffer,g_MSCore.GetEngineUtils()->GetPlayerName(g_LastPlayerIndex)) != NULL)
		PlayerNameIndex = g_LastPlayerIndex;
	else
	{
		for (int i=1;i<g_MSCore.GetEngineUtils()->MaxClients;i++) if(g_MSCore.GetEngineUtils()->FastIsUserConnected(i))
		{
			const char *StrFound = strstr(Buffer,g_MSCore.GetEngineUtils()->GetPlayerName(i));
			if(StrFound)
			{
				PlayerNameIndex = i;
				break;
			}
		}
	}


	if(PlayerNameIndex == -1)
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send private message with a colored nickname with a nick that dont exist");
		PreSamplerEnd("SrvConsoleCmds::CmdPSayViaChat")
		return;
	}
	
	const char *pPlayerNick = g_MSCore.GetEngineUtils()->GetPlayerName(PlayerNameIndex);
	int StrLen = strlen(pPlayerNick);

	g_MSCore.GetEngineUtils()->MessagePlayer2(id,"\x03%s\x01 %s",pPlayerNick,Buffer + StrLen );
	PreSamplerEnd("SrvConsoleCmds::CmdPSayViaChat")
}
#if ORANGEBOX_ENGINE == 1
void SrvConsoleCmds::CmdPSay2ViaChat(const CCommand &args)
{
	g_LastCCommand = args;
#else
void SrvConsoleCmds::CmdPSay2ViaChat()
{
#endif
	// Tobi should shot for doing this. every time someone sends place command the remote server a list of ms_psay for every player
	//ms_psay "10" -[SF]- Viper is on rank 169 of 143,407 with 6,495 points;
	//ms_psay "11" -[SF]- Viper is on rank 169 of 143,407 with 6,495 points;
	//ms_psay "8" -[SF]- Viper is on rank 169 of 143,407 with 6,495 points;"
	PreSamplerStart("SrvConsoleCmds::CmdPSay2ViaChat")
	char Buffer[240];

	int ArgCount = g_LastCCommand.ArgC();
	int len=0;
	int id = g_MSCore.GetEngineUtils()->FindPlayer(atoi(g_LastCCommand.Arg(1)));

	if(id == -1 || id == 0)
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send private message but did not pass a valid userid");
		PreSamplerEnd("SrvConsoleCmds::CmdPSay2ViaChat")
		return;
	}

	if(ArgCount >= 2)
	{
		len += _snprintf(&(Buffer[len]),239-len,"%s",g_LastCCommand.Arg(2));

		for(int i=3;i<ArgCount;i++)
		{
			len += _snprintf(&(Buffer[len]),239-len," %s",g_LastCCommand.Arg(i));
		}
	}
	else
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send private message but did not contain a message");
		PreSamplerEnd("SrvConsoleCmds::CmdPSay2ViaChat")
		return;
	}

	//Now we need to find whats the player index is, from his nick name
	int PlayerNameIndex = -1;

	if(g_LastPlayerIndex != -1 && strstr(Buffer,g_MSCore.GetEngineUtils()->GetPlayerName(g_LastPlayerIndex)) != NULL)
		PlayerNameIndex = g_LastPlayerIndex;
	else
	{
		for (int i=1;i<g_MSCore.GetEngineUtils()->MaxClients;i++) if(g_MSCore.GetEngineUtils()->FastIsUserConnected(i))
		{
			const char *StrFound = strstr(Buffer,g_MSCore.GetEngineUtils()->GetPlayerName(i));
			if(StrFound)
			{
				PlayerNameIndex = i;
				break;
			}
		}
	}

	if(PlayerNameIndex == -1)
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send private message with a colored nickname with a nick that dont exist");
		PreSamplerEnd("SrvConsoleCmds::CmdPSay2ViaChat")
		return;
	}

	const char *pPlayerNick = g_MSCore.GetEngineUtils()->GetPlayerName(PlayerNameIndex);
	int StrLen = strlen(pPlayerNick);

	g_MSCore.GetEngineUtils()->MessagePlayer(id,"\04%s",Buffer);
	PreSamplerEnd("SrvConsoleCmds::CmdPSay2ViaChat")
}

#if ORANGEBOX_ENGINE == 1
void SrvConsoleCmds::CmdMenuSay(const CCommand &args)
{
	g_LastCCommand = args;
#else
void SrvConsoleCmds::CmdMenuSay()
{
#endif	

	PreSamplerStart("SrvConsoleCmds::CmdMenuSay")
	unsigned int g_ArgCount = g_LastCCommand.ArgC();
	char MenuString[MaxMenuSize + 1];

	int len=0;
	int id = g_MSCore.GetEngineUtils()->FindPlayer(atoi(g_LastCCommand.Arg(2)));

	if(g_ArgCount >= 3)
	{
		len += _snprintf(&(MenuString[len]),MaxMenuSize-len,"%s",g_LastCCommand.Arg(3));
		
		for(unsigned int i=4;i<g_ArgCount;i++)
		{			
			len += _snprintf(&(MenuString[len]),MaxMenuSize-len," %s",g_LastCCommand.Arg(i));
		}
	}
	else
		_snprintf(MenuString,MaxMenuSize,"%s",g_LastCCommand.ArgS());

	if(id == -1)
	{
		g_MSCore.GetEngineUtils()->LogPrint("Tried to send menu message but did not valid userid");
		PreSamplerEnd("SrvConsoleCmds::CmdMenuSay")
		return;
	}

	bool LastWasSign=false;
	int FoundCount = 0;

	for(int i=0;i<len;i++)
	{
		if(MenuString[i] == 92) // this is "\"
			LastWasSign = true; // 110 is n
		else if(LastWasSign && MenuString[i] == 110)
		{
			FoundCount++;
			LastWasSign=false;

			MenuString[i-1]  = '#';
			MenuString[i]  = '#';
		}
	}
	int test = g_MSCore.StrReplace(MenuString,"##","#",MaxMenuSize);
	for(int i=0;i<len;i++)
	{
		if(MenuString[i] == '#')
			MenuString[i] = '\n';
	}

	BATMenuMngr::ShowRadioMenu(id,MenuString);
	PreSamplerEnd("SrvConsoleCmds::CmdMenuSay")
}

#if ORANGEBOX_ENGINE == 1
void SrvConsoleCmds::CmdSwapUserTeam(const CCommand &args)
{
	g_LastCCommand = args;
#else
void SrvConsoleCmds::CmdSwapUserTeam()
{
#endif
	PreSamplerStart("SrvConsoleCmds::CmdSwapUserTeam")
	int ArgCount = g_LastCCommand.ArgC();
	int len=0;
	int id = g_MSCore.GetEngineUtils()->FindPlayer(atoi(g_LastCCommand.Arg(1)));
	if(id == -1)
		return;

	IPlayerInfo *pPlayerInfo = g_MSCore.GetPlayerInfo()->GetPlayerInfo(g_UserInfo[id].PlayerEdict);

	if (pPlayerInfo && g_UserInfo[id].PlayerEdict && !g_UserInfo[id].PlayerEdict->IsFree()) 
	{ 
		int Team = pPlayerInfo->GetTeamIndex();
		if(Team == 2)
			Team = 3;
		else if(Team == 3)
			Team = 2;
		else
		{
			PreSamplerEnd("SrvConsoleCmds::CmdSwapUserTeam")
			return;
		}

		pPlayerInfo->ChangeTeam( Team ); 
	}
	PreSamplerEnd("SrvConsoleCmds::CmdSwapUserTeam")
}

void SrvConsoleCmds::ShowMOTD(int id,const char *url)
{
	RecipientFilter rf; // the corresponding recipient filter 

	rf.AddPlayer (id); 
	rf.MakeReliable();

	bf_write *netmsg = g_MSCore.GetEngine()->UserMessageBegin (static_cast<IRecipientFilter *>(&rf), g_ModSettings.VGUIMenu); 
	netmsg->WriteString("info"); // the HUD message itself 
	netmsg->WriteByte(1);
	netmsg->WriteByte(3); // I don't know yet the purpose of this byte, it can be 1 or 0 
	netmsg->WriteString("type"); // the HUD message itself 
	netmsg->WriteString("2"); // the HUD message itself 
	netmsg->WriteString("title"); // the HUD message itself 
	netmsg->WriteString("Server Rules"); // the HUD message itself 

	netmsg->WriteString("msg"); // the HUD message itself 
	netmsg->WriteString(url); // the HUD message itself 

	g_MSCore.GetEngine()->MessageEnd();
}