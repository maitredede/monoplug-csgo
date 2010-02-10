#ifndef _CMONOPLAYER_H_
#define _CMONOPLAYER_H_

#include "monoBase.h"
#include "pluginterfaces.h"

namespace MonoPlugin
{
	class CMonoPlugin;

	class CMonoPlayer
	{
	private:
		MonoObject* m_obj;
		bool m_isBot;
		int m_userId;
		char m_name[MAX_PLAYER_NAME_LENGTH];
	public:
		CMonoPlayer(int userId, bool isBot);
		//~CMonoPlayer();
		MonoObject* GetPlayer();

		void SetConnected(bool connecting, bool connected);
		void SetData(const char* name, const char* address);
		void SetSteam(bool valid, const char* networkid);
		const char* Name();
		
		bool IsBot();
		int UserId();
	};

	typedef struct
	{
		edict_t *PlayerEdict;
		//bool IsBot;						// True if its a BOT or a hltv
		CMonoPlayer* Player;
	} CMonoPlayerInfo;

}

#endif //_CMONOPLAYER_H_
