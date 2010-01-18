/* ======== Basic Admin tool ========
* Copyright (C) 2004-2008 Erling K. Sæterdal
* No warranties of any kind
*
* License: zlib/libpng
*
* Author(s): Erling K. Sæterdal ( EKS )
* Credits:
*	Menu code based on code from CSDM ( http://www.tcwonline.org/~dvander/cssdm ) Created by BAILOPAN
*	Helping on misc errors/functions: BAILOPAN,karma,LDuke,sslice,devicenull,PMOnoTo,cybermind ( most who idle in #sourcemod on GameSurge realy )
* ============================ */

#include "BATMenu.h"
#include "MiniStatsCore.h"
#include "keyvalues.h"
#include "recipientfilters.h"
#include "usermessages.h"
#include "color.h"
#include "const.h"
#include "sh_string.h"
#include <sh_vector.h>
#include <bitbuf.h>

#define PluginBase PluginCore

BATMenuBuilder *g_MenuBuilder;


// -- Used when reshowing menus in Orangebox
bool g_RequiresMenuReShow;
bool g_IsMenuReShow;
char g_OldMenu[MAX_PLAYERS][MaxMenuSize+1]; // Used to resend the menus
int g_OldMenuShowTimes[MAX_PLAYERS];
// -- 
char g_MenuTitle[MAX_MENUOPTIONLENGTH];
char g_MenuOption[10][MAX_MENUOPTIONLENGTH+1];
int g_MenuKeys;
int g_ActiveOptions;

void BATMenuBuilder::AddOption(const char *fmt,eMenuSelectionColor Color)
{
	if(g_ActiveOptions >= 10)
	{
		PluginCore.PluginSpesficLogPrint("!ERROR! To many options added to a menu, '%s' was ignored",fmt);
		return;
	}	

	switch(Color)
	{
	case MENUSELECTION_COLOR_WHITE:
		_snprintf(g_MenuOption[g_ActiveOptions],MAX_MENUOPTIONLENGTH,"%s",fmt);
		break;

	case MENUSELECTION_COLOR_YELLOW:
		_snprintf(g_MenuOption[g_ActiveOptions],MAX_MENUOPTIONLENGTH,"%s",fmt);
		break;
	}
	
	g_ActiveOptions++;
}
void BATMenuBuilder::SetTitle(const char *title,eMenuSelectionColor Color)
{
	switch(Color)
	{
	case MENUSELECTION_COLOR_WHITE:
		_snprintf(g_MenuTitle,MAX_MENUOPTIONLENGTH,"%s",title);
		break;

	case MENUSELECTION_COLOR_YELLOW:
		_snprintf(g_MenuTitle,MAX_MENUOPTIONLENGTH,"%s",title);
		break;
	}	
}
void BATMenuBuilder::SetKeys(int keys)
{
	g_MenuKeys = keys;
}
int BATMenuMngr::RegisterMenu(IMenu *menu)
{
	g_MenuList.push_back(new stMenu);

	g_MenuList[g_ActiveMenus]->menu = menu;
	g_ActiveMenus++;

	return (int)(g_ActiveMenus-1);
}

void BATMenuMngr::ShowMenu(int player, menuId menuid)
{
	g_UserInfoMenu[player].index = player;
	g_UserInfoMenu[player].menu = menuid;
	g_HasMenuOpen[player] = true;

	bool MenuGoodChoice = g_MenuList[menuid]->menu->Display(g_MenuBuilder,player);
	if(!MenuGoodChoice)
		return;

	char MenuMsg[MaxMenuSize+1];
	int len=0;

	if(g_MenyType == 1)
	{
		RecipientFilter rf;

		if (player == 0) 
		{
			for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++) if(PluginCore.GetEngineUtils()->FastIsUserConnected(i) && !g_UserInfo[i].IsBot)
			{
				g_UserInfoMenu[i].index = i;
				g_UserInfoMenu[i].menu = menuid;
				g_HasMenuOpen[i] = true;
				rf.AddPlayer(i);
			}
		} else {
			rf.AddPlayer(player);
		}
		rf.MakeReliable();

		len += _snprintf(MenuMsg,MaxMenuSize,"%s\n",g_MenuTitle);
		for(int i=0;i<g_ActiveOptions;i++)
		{
			if(strlen(g_MenuOption[i]) == 0 || g_MenuOption[i][0] == '\n')
				len += _snprintf(&(MenuMsg[len]),MaxMenuSize-len,"\n");
			else
				len += _snprintf(&(MenuMsg[len]),MaxMenuSize-len,"%d. %s\n",i+1,g_MenuOption[i]);
		}
		ShowRadioMenu(player,MenuMsg);		
	}
	else
	{
		for(int i=MAX_MENUOPTIONLENGTH-1;i > 0;i--)
		{
			if(g_MenuTitle[i] == ':')
			{
				g_MenuTitle[i] = '\0';
				break;
			}
		}

		KeyValues *kv = new KeyValues( "menu" );
		kv->SetString( "title", g_MenuTitle );
		kv->SetInt( "level", 1 );
		kv->SetColor( "color", Color( 255, 0, 0, 255 ));
		kv->SetInt( "time", 20 );
		kv->SetString( "msg", "Make your selection in the menu" );

		char num[11], msg[MAX_MENUOPTIONLENGTH+1], cmd[MAX_MENUOPTIONLENGTH+1];

		for(int i=0;i<g_ActiveOptions;i++)
		{
			if(strlen(g_MenuOption[i]) == 0 || g_MenuOption[i][0] == '\n') // i >= g_ActiveOptions || 
				continue;
			
			_snprintf( num, 10, "%i", i );
			_snprintf( cmd, MAX_MENUOPTIONLENGTH, "menuselect %i", i+1 );
			_snprintf( msg, MAX_MENUOPTIONLENGTH, "%i. %s", i+1,g_MenuOption[i] );

			KeyValues *item1 = kv->FindKey( num, true );
			item1->SetString( "msg", msg );
			item1->SetString( "command", cmd );
		}
		if(player == 0)
		{
			for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++) if(PluginCore.GetEngineUtils()->FastIsUserConnected(i) && !g_UserInfo[i].IsBot)
			{
				g_UserInfoMenu[i].index = i;
				g_UserInfoMenu[i].menu = menuid;
				g_HasMenuOpen[i] = true;
				PluginCore.GetHelpers()->CreateMessage( g_UserInfo[i].PlayerEdict, DIALOG_MENU, kv, this );
			}
		}
		else
			PluginCore.GetHelpers()->CreateMessage( g_UserInfo[player].PlayerEdict, DIALOG_MENU, kv, this );

		kv->deleteThis();
	}
	g_ActiveOptions=0;
}


void BATMenuMngr::MenuChoice(int playerIndex, int choice)
{
	menuId menuid = g_UserInfoMenu[playerIndex].menu;
	g_HasMenuOpen[playerIndex] = false;

	switch(g_MenuList[menuid]->menu->MenuChoice(g_UserInfoMenu[playerIndex], choice))
	{
	case MENUSELECTION_GOOD:
		PluginCore.GetEngine()->ClientCommand(g_UserInfo[playerIndex].PlayerEdict,PLAYSOUND_GOOD);
		break;
	case MENUSELECTION_BAD:
		PluginCore.GetEngine()->ClientCommand(g_UserInfo[playerIndex].PlayerEdict,PLAYSOUND_BAD);
	    break;
	}
}

void BATMenuMngr::ShowRadioMenu(int id,char *MenuString)
{
	if(g_RequiresMenuReShow && !g_IsMenuReShow)
	{
		Q_snprintf(g_OldMenu[id],MaxMenuSize,"%s",MenuString);
		g_OldMenuShowTimes[id] = MENU_RESHOW_TIMES;
		//PluginCore.GetTaskSystem()->CreateTask(BATMenuMngr::cbReOldMenuToPlayer,MENU_RESHOW_TIME,id);
	}

	RecipientFilter rf;
	
	if (id == 0) 
	{
		rf.AddAllPlayers(PluginCore.GetEngineUtils()->MaxClients);
	} else {
		rf.AddPlayer(id);
	}
	rf.MakeReliable();

	char *ptr = MenuString;
	size_t len = strlen(MenuString);
	char save = 0;

	while (true)
	{
		if (len > MaxPrSend)
		{
			save = ptr[MaxPrSend];
			ptr[MaxPrSend] = '\0';
		}
		bf_write *buffer = PluginCore.GetEngine()->UserMessageBegin(static_cast<IRecipientFilter *>(&rf), g_ModSettings.MenuMsg);
		buffer->WriteWord((1<<0) | (1<<1) | (1<<2) | (1<<3) | (1<<4) | (1<<5) | (1<<6) | (1<<7)| (1<<8) | (1<<9));
		buffer->WriteChar(MENU_OPEN_TIME);
		buffer->WriteByte( (len > MaxPrSend) ? 1 : 0 );
		buffer->WriteString(ptr);
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

void BATMenuMngr::ResetPlayer(int id)
{
	g_HasMenuOpen[id] = false;
	g_OldMenuShowTimes[id] = 0;

	for (uint i=0;i<g_ActiveMenus;i++)
	{
		g_MenuList[i]->menu->ResetPlayer(id);
	}
}
void BATMenuMngr::ClearForMapchange()
{
	for(int i=1;i<=PluginCore.GetEngineUtils()->MaxClients;i++)
	{
		ResetPlayer(i);
	}
}
void BATMenuMngr::ShowCornerMsg(int id,const char *TheMsg)
{
	KeyValues *kv = new KeyValues( "msg" );
	kv->SetString( "title", TheMsg );
	kv->SetColor( "color", Color( 255, 0, 0, 255 ));
	kv->SetInt( "level", 5);
	kv->SetInt( "time", 10);
	PluginCore.GetHelpers()->CreateMessage( g_UserInfo[id].PlayerEdict, DIALOG_MSG, kv, this );
	kv->deleteThis();
}

void BATMenuMngr::ReShowOldMenus()
{
	for (int i=0;i<PluginCore.GetEngineUtils()->MaxClients;i++)
	{
		if(!PluginCore.GetEngineUtils()->FastIsUserConnected(i) || g_OldMenuShowTimes[i] == 0)
			continue;

		g_OldMenuShowTimes[i]--;
		g_IsMenuReShow = true;
		ShowRadioMenu(i,g_OldMenu[i]);
		g_IsMenuReShow = false;
	}
}

void BATMenuMngr::cbReOldMenuToPlayer(int TaskID)
{
	if(g_OldMenuShowTimes[TaskID] > 0)
	{
		g_OldMenuShowTimes[TaskID]--;
		g_IsMenuReShow = true;
		ShowRadioMenu(TaskID,g_OldMenu[TaskID]);
		g_IsMenuReShow = false;
	}
}


#ifndef DIALOG_ASKCONNECT
#define DIALOG_ASKCONNECT 4		// Im not updating all the sdk around so we do this.
#endif

void BATMenuMngr::ShowClientReConnectDialog(int id)
{

	
}
#if BAT_DEBUG == 1
/*
void BATMenuMngr::ShowESCTextBox(int id)
{
	KeyValues *kv = new KeyValues( "menu" );
	kv->SetString( "title", "You've got options, hit ESC" );
	kv->SetInt( "level", 1 );
	kv->SetColor( "color", Color( 255, 0, 0, 255 ));
	kv->SetInt( "time", 20 );
	kv->SetString( "msg", "Pick an option\nOr don't." );

	PluginCore.GetHelpers()->CreateMessage( g_UserInfo[id].PlayerEdict, DIALOG_TEXT, kv, this );
	kv->deleteThis();
}
*/
#endif