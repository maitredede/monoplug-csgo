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

#ifndef _INCLUDE_HLXMenu
#define _INCLUDE_HLXMenu

#include "BATMenu.h"

//extern menuId g_AdminMenu;
class HLXMenu : public IMenu
{
public:
	bool Display(BATMenuBuilder *make, int playerIndex);
	eMenuSelectionType MenuChoice(stPlayer player, int option);
	void ResetPlayer(int id);

private:
	
	int g_PlayerMenuPage[MAXPLAYERS+1];
};
#endif // _INCLUDE_HLXMenu