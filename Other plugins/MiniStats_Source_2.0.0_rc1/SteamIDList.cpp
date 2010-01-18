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

#include "SteamIDList.h"
#include <sourcehook/sourcehook.h>
#include <sh_vector.h>
#include <time.h>

/**********************	Public functions **********************/
void SteamIDList::AddSteamID(const char *pSteamID)
{
	if(g_SteamIDListCount >= g_SteamIDList.size()-1 || g_SteamIDList.size() == 0)
		GrowSteamIDList();

	//STEAM_0:0:78039
	stSteamIDInfo tSteamid = GetstSteamIDInfo(pSteamID);
	if(tSteamid.SteamIDNum == 0) // Something went wrong parsing that steamid
	{
		g_MSCore.GetEngineUtils()->LogPrint("[MS] Failed to parse steamid %s for client",pSteamID);
		return;
	}
	_snprintf(g_SteamIDList[g_SteamIDListCount]->SteamID,MAX_NETWORKID_LENGTH,"%s",pSteamID);
	g_SteamIDList[g_SteamIDListCount]->FirstNum = tSteamid.FirstNum;
	g_SteamIDList[g_SteamIDListCount]->SecNum = tSteamid.SecNum;
	g_SteamIDList[g_SteamIDListCount]->ThirdNum = tSteamid.ThirdNum;
	g_SteamIDList[g_SteamIDListCount]->SteamIDNum = tSteamid.SteamIDNum;
	g_SteamIDList[g_SteamIDListCount]->DaysSizeLastUse = GetDaysSince1970();
	g_SteamIDListCount++;
}

void SteamIDList::RemoveSteamID(const char *pSteamID)
{
	stSteamIDInfo tSteamid = GetstSteamIDInfo(pSteamID);
	if(tSteamid.SteamIDNum ==0) // Something went wrong parsing that steamid
	{
		g_MSCore.GetEngineUtils()->LogPrint("[MS] Failed to parse steamid %s for client",pSteamID);
		return;
	}
	
	SourceHook::CVector<stSteamIDInfo *>::iterator i;
	for(i=g_SteamIDList.begin();i != g_SteamIDList.end(); i++)
	{
		stSteamIDInfo *tSteamid2 = (*i);
		if(tSteamid.SteamIDNum == tSteamid2->SteamIDNum && tSteamid.FirstNum == tSteamid2->FirstNum  && tSteamid.SecNum == tSteamid2->SecNum && tSteamid.ThirdNum == tSteamid2->ThirdNum)
		{
			g_SteamIDList.erase(i);
			break;
		}
	}
}

bool SteamIDList::IsSteamIDInList(const char *pSteamID)
{
	stSteamIDInfo tSteamid = GetstSteamIDInfo(pSteamID);
	if(tSteamid.SteamIDNum ==0) // Something went wrong parsing that steamid
	{
		g_MSCore.GetEngineUtils()->LogPrint("[MS] Failed to parse steamid %s for client",pSteamID);
		return false;
	}
	if(g_SteamIDListCount == 0)
		return false;
	
	for(uint i=0;i<g_SteamIDListCount;i++)
	{
		if(tSteamid.SteamIDNum == g_SteamIDList[i]->SteamIDNum && tSteamid.FirstNum == g_SteamIDList[i]->FirstNum  && tSteamid.SecNum == g_SteamIDList[i]->SecNum && tSteamid.ThirdNum == g_SteamIDList[i]->ThirdNum)
		{
			g_SteamIDList[i]->DaysSizeLastUse = GetDaysSince1970();
			return true;
		}
	}
	return false;
}

void SteamIDList::ReadSteamIDFile() 
{
	//Begin loading admins
	char temp[786], tdir[512],Line[512];	//This gets the actual path to the users.ini file.
	char *pLine;

	g_MSCore.GetEngine()->GetGameDir(tdir,512);
	_snprintf(temp,785,STEAMID_FILE,tdir);
	FILE *is = NULL;
	is = fopen(temp,"rt");

	if (!is)
	{
		_snprintf(temp,785,"ERROR: Unable to find Old steamid file, should be in ( %s )\n",STEAMID_FILE);
		g_MSCore.GetEngine()->LogPrint(temp);
		return;
	}

	int TextIndex=0;
	int LineNum=0;
	int iLineLen;

	int IOT;
	int TempDays;

	int CurDaysSize1970 = GetDaysSince1970();
	int OldSteamIDRemoved = 0;
	g_SteamIDList.clear(); // Remove any old entries

	while (!feof(is)) 
	{
		if(g_SteamIDListCount >= g_SteamIDList.size()-1 || g_SteamIDList.size() == 0)
			GrowSteamIDList();
		

		fgets(Line,512,is);
		iLineLen = strlen(Line);
		LineNum++;
		
		if(iLineLen <= 5 || Line[0] == ';' ||  Line[0] == '/')
			continue;
		
		IOT = GetFirstIndexOfChar(Line,MAX_NETWORKID_LENGTH,' ');
		if(IOT == -1)
			return;

		_snprintf(temp,IOT,"%s",Line);
		temp[IOT] = '\0';
		
		pLine = (char *)Line;
		pLine = pLine + IOT;
		TempDays = atoi(pLine);

		if((CurDaysSize1970-TempDays) <= HLX_REMOVEOLDSTEAMIDS)
			AddSteamID(temp,TempDays);
		else
		{
#if HLX_DEBUG == 1
			_snprintf(tdir,511,"HLX Removed %s was %d days old\r\n",temp,(CurDaysSize1970-TempDays));
			g_MSCore.GetEngine()->LogPrint(tdir);
#endif
			OldSteamIDRemoved++;
		}
	}
	fclose(is);

	if(OldSteamIDRemoved > 0)
	{
		_snprintf(tdir,511,"HLX: Removed %d old steamids from list\r\n",OldSteamIDRemoved,(CurDaysSize1970-TempDays));
		g_MSCore.GetEngine()->LogPrint(tdir);
	}
}

void SteamIDList::WriteSteamIDFile()
{
	char Temp[786], tdir[512];

	g_MSCore.GetEngine()->GetGameDir(tdir,512);
	_snprintf(Temp,785,STEAMID_FILE,tdir);

	FILE *FileStream = fopen(Temp, "wt"); // "wt" clears the file each time
	
	if (!FileStream)
	{
		_snprintf(Temp,511,"ERROR: Unable to write steamid file, should be in ( %s )",STEAMID_FILE);
		g_MSCore.GetEngine()->LogPrint(Temp);
		return;
	}
	
	SourceHook::CVector<stSteamIDInfo *>::iterator i;
	for(i=g_SteamIDList.begin();i != g_SteamIDList.end(); i++)
	{
		stSteamIDInfo *tSteamid2 = (*i);
		
		if(tSteamid2->DaysSizeLastUse == -1)
			continue;

		_snprintf(Temp,511,"%s %d\r\n",tSteamid2->SteamID,tSteamid2->DaysSizeLastUse);
		fputs(Temp,FileStream);
	}
	fclose(FileStream);
}
/**********************	Private functions **********************/
stSteamIDInfo SteamIDList::GetstSteamIDInfo(const char *SteamID)
{
	if(g_SteamIDListCount >= g_SteamIDList.size()-1 || g_SteamIDList.size() == 0)
		GrowSteamIDList();

	//STEAM_0:0:78039
	stSteamIDInfo tSteamid;
	tSteamid.SteamIDNum = 0; // If we return this stSteamIDInfo like this, the other functions should check for the 0, and if it has it. Then something went wrong
	char tText[MAX_NETWORKID_LENGTH];
	_snprintf(tSteamid.SteamID,MAX_NETWORKID_LENGTH,"%s",SteamID);

	int SkipInt = 6; // this removes the STEAM_

	int IOT = GetFirstIndexOfChar(SteamID+SkipInt,MAX_NETWORKID_LENGTH-(SkipInt+1),':');	
	if(IOT <= 0)
		return tSteamid;

	_snprintf(tText,IOT,"%s",SteamID+SkipInt);
	tText[IOT]  = '\0';
	tSteamid.FirstNum = atoi(tText);

	SkipInt += IOT + 1;
	IOT = GetFirstIndexOfChar(SteamID+SkipInt,MAX_NETWORKID_LENGTH-(SkipInt+1),':');
	if(IOT <= 0)
		return tSteamid;

	_snprintf(tText,IOT,"%s",SteamID+SkipInt);
	tText[IOT] = '\0';
	tSteamid.SecNum = atoi(tText);

	SkipInt += IOT + 1;
	_snprintf(tText,MAX_NETWORKID_LENGTH,"%s",SteamID+SkipInt);
	tSteamid.ThirdNum = atoi(tText);

	// The steamid is now nice and split up.
	tSteamid.SteamIDNum = tSteamid.FirstNum + tSteamid.SecNum + tSteamid.ThirdNum;

	return tSteamid;
}
void SteamIDList::GrowSteamIDList()
{
	stSteamIDInfo *tSteamid = new stSteamIDInfo;
	tSteamid->DaysSizeLastUse = -1;
	g_SteamIDList.push_back(tSteamid);
}
int SteamIDList::GetDaysSince1970()
{
	time_t seconds;
	seconds = time (NULL);

	return  seconds/86400;
}
void SteamIDList::AddSteamID(const char *SteamID,uint DaysSinceOnline)
{
	if(g_SteamIDListCount >= g_SteamIDList.size()-1 || g_SteamIDList.size() == 0)
		g_SteamIDList.push_back(new stSteamIDInfo);

	//STEAM_0:0:78039
	stSteamIDInfo tSteamid = GetstSteamIDInfo(SteamID);
	if(tSteamid.SteamIDNum ==0) // Something went wrong parsing that steamid
	{
		g_MSCore.GetEngineUtils()->ServerCommand("echo ERROR parsing steamid: %s",SteamID);
		return;
	}
	_snprintf(g_SteamIDList[g_SteamIDListCount]->SteamID,MAX_NETWORKID_LENGTH,"%s",SteamID);
	g_SteamIDList[g_SteamIDListCount]->FirstNum = tSteamid.FirstNum;
	g_SteamIDList[g_SteamIDListCount]->SecNum = tSteamid.SecNum;
	g_SteamIDList[g_SteamIDListCount]->ThirdNum = tSteamid.ThirdNum;
	g_SteamIDList[g_SteamIDListCount]->SteamIDNum = tSteamid.SteamIDNum;
	g_SteamIDList[g_SteamIDListCount]->DaysSizeLastUse = DaysSinceOnline;
	g_SteamIDListCount++;
}