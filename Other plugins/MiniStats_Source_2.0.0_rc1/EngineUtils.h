#ifndef _INCLUDE_EngineUtils_CLASS
#define _INCLUDE_EngineUtils_CLASS

#include "const.h"
#include <string.h>
#include <strtools.h>
#include <string.h>
#include <bitbuf.h>
#include <ctype.h>
#include <eiface.h>
#include <iplayerinfo.h>
#include "recipientfilters.h"

class EngineUtils
{
public:
	EngineUtils(IVEngineServer *pEngine,IPlayerInfoManager *pInfoMngr) {m_Engine = pEngine;m_InfoMngr = pInfoMngr;}


	void MessagePlayer(int index, const char *msg, ...);
	void MessagePlayer2(int index, const char *msg, ...);
	void SendCSay(int id,const char *msg, ...);
	void ServerCommand(const char *fmt, ...);
	void SendHintMsg(int id,char *text);
	void SendHintMsgNoFormat(int id,char *text);
	void LogPrint( const char *msg, ...);
	int GetUserTeam(int id);
	int FindPlayer(int UserID);
	int FindPlayer(const char *Name);
	void FakeClientSay(int id, const char *msg, ...);
	const char *GetTeamName(int id);
	const char *GetPlayerName(int id);
	const char *GetPlayerWeapon(int id);
	const char *GetPlayerWeaponNamePretty(int id);

	const Vector GetPlayerOrigin(int id);

	bool FastIsUserConnected(int id) {return g_IsConnected[id];}
	void FastSetUserConnected(int id,bool NewStatus) {g_IsConnected[id] = NewStatus;}

	//void SetEnginePointer(IVEngineServer *pEngine,IPlayerInfoManager *pInfoMngr) {m_Engine = pEngine;m_InfoMngr = pInfoMngr;}

	int MaxClients;

private:
	int GetNextSpaceCount(const char *Text,int StartIndex,int Maxlen);

	bool g_IsConnected[MAXPLAYERS+1];

	IVEngineServer *m_Engine;
	IPlayerInfoManager *m_InfoMngr;
};
#endif //EngineUtils