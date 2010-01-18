/* ======== Basic Admin tool ========
* Copyright (C) 2004-2007 Erling K. Sæterdal
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): Erling K. Sæterdal ( EKS )
* Credits:
*	Menu code based on code from CSDM ( http://www.tcwonline.org/~dvander/cssdm ) Created by BAILOPAN
*	Adminloading function from Xaphan ( http://www.sourcemod.net/forums/viewtopic.php?p=25807 ) Created by Devicenull
* Helping on misc errors/functions: BAILOPAN,sslice,devicenull,PMOnoTo,cybermind ( most who idle in #sourcemod on GameSurge realy )
* ============================ */

#include "MiniStatsCore.h"
#include "HLXMenu.h"
#include "BATMenu.h"

extern BATMenuMngr *m_MenuMngr;

bool HLXMenu::Display(BATMenuBuilder *make, int playerIndex)
{
	make->SetKeys( (1<<0) | (1<<1) | (1<<2) | (1<<3) | (1<<4) | (1<<5) | (1<<6) | (1<<7)| (1<<8) | (1<<9));

	switch(g_PlayerMenuPage[playerIndex])
	{
	default:
	case 0:
		make->SetTitle("HLStatsX ingame menu",MENUSELECTION_COLOR_YELLOW);
		make->AddOption("Display Rank",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Next Players",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Top10 Players",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Clans Ranking",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Server Status",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Statsme",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Auto Ranking",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Console Events",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Next page",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Exit",MENUSELECTION_COLOR_WHITE);
		break;
	
	case 1:
		make->SetTitle("HLStatsX ingame menu",MENUSELECTION_COLOR_YELLOW);
		make->AddOption("Weapon Usage",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Weapons Accuracy",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Weapons Targets",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Player Kills",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Toggle Ranking Display",MENUSELECTION_COLOR_WHITE);
		make->AddOption("VAC Cheaterlist",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Display Help",MENUSELECTION_COLOR_WHITE);
		break;

	case 2:
		make->SetTitle("HLstatsX - Auto-Ranking",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Enable on round-start",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Enable on round-end",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Enable on player death",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Disable",MENUSELECTION_COLOR_WHITE);
	    break;

	case 3:
		make->SetTitle("HLstatsX - Console Events",MENUSELECTION_COLOR_YELLOW);
		make->AddOption("Enable Events",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Disable Events",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Enable Global Chat",MENUSELECTION_COLOR_WHITE);
		make->AddOption("Disable Global Chat",MENUSELECTION_COLOR_WHITE);
	    break;

	}

	return true;
}

eMenuSelectionType HLXMenu::MenuChoice(stPlayer player, int option)
{
	int id = player.index;

	if(g_PlayerMenuPage[id] == 0)
	{
		switch(option)
		{
		case 1:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/rank");
			break;
		case 2:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/next");
			break;
		case 3:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/top10");
		    break;

		case 4:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/clans");
		    break;

		case 5:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/status");
			break;

		case 6:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/statsme");
			break;

		case 7:
			g_PlayerMenuPage[id] = 2; // Auto ranking menu
			m_MenuMngr->ShowMenu(id,player.menu);
		    break;

		case 8:
			g_PlayerMenuPage[id] = 3; // Coneole menu
			m_MenuMngr->ShowMenu(id,player.menu);
		    break;

		case 9: 
			g_PlayerMenuPage[id] = 1;
			m_MenuMngr->ShowMenu(id,player.menu);
			break;
		}
	}
	else if(g_PlayerMenuPage[id] == 1)
	{
		switch(option)
		{
		case 1:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/accuracy");
			break;

		case 2:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/targets");
			break;
		case 3:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/kills");
			break;
		case 4:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_hideranking");
			break;

		case 5:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/cheaters");
			break;

		case 6:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/help");
			break;
		}
		g_PlayerMenuPage[id] =0;		
	}
	else if(g_PlayerMenuPage[id] == 2)
	{
		switch(option)
		{
		case 1:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_auto start rank");
			break;
		case 2:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_auto end rank");
			break;
		case 3:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_auto kill rank");
			break;

		case 4:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_auto clear");
			break;
		}
		g_PlayerMenuPage[id] =0;		
	}
	else if(g_PlayerMenuPage[id] == 3)
	{
		switch(option)
		{
		case 1:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_display 1");
			break;
		case 2:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_display 0");
			break;
		case 3:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_chat 1");
			break;

		case 4:
			g_MSCore.GetEngineUtils()->FakeClientSay(id,"/hlx_chat 0");
			break;
		}
		g_PlayerMenuPage[id] =0;		
	}
	return MENUSELECTION_GOOD;
}

void HLXMenu::ResetPlayer(int id)
{
	g_PlayerMenuPage[id] =0;
}