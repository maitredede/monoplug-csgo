/* ======== SimpleStats ========
* Copyright (C) 2004-2008 Erling K. Sæterdal
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

#ifndef _INCLUDE_SRVCONSOLECMDS
#define _INCLUDE_SRVCONSOLECMDS

#include "MiniStatsCore.h"

//extern menuId g_AdminMenu;
class SrvConsoleCmds
{
public:
#if ORANGEBOX_ENGINE == 1
	static void CmdHintMsg(const CCommand &args);
	static void CmdPSayViaChat(const CCommand &args);
	static void CmdPSay2ViaChat(const CCommand &args);
	static void CmdCenterSay(const CCommand &args);
	static void CmdMenuSay(const CCommand &args);
	static void CmdShowMOTD(const CCommand &args);
	static void CmdSwapUserTeam(const CCommand &args);
#else
	static void CmdHintMsg();
	static void CmdPSayViaChat();
	static void CmdPSay2ViaChat();
	static void CmdCenterSay();
	static void CmdMenuSay();
	static void CmdShowMOTD();
	static void CmdSwapUserTeam();
#endif

	static void SendMenuString(int id,char *MenuString);
	static void ShowMOTD(int target_id,const char *url);
private:
	

};
#endif