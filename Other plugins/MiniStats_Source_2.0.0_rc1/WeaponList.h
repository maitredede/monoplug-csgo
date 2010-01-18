#ifndef _INCLUDE_WeaponList_CLASS
#define _INCLUDE_WeaponList_CLASS

#include <string.h>
#include <strtools.h>
#include <string.h>
#include <bitbuf.h>
#include <ctype.h>
#include <eiface.h>

class WeaponList
{
public:
	~WeaponList() {g_WeaponListCount = 0; };
	WeaponList() {g_WeaponListCount = 0; };

	int GetWeaponIndex(const char *WeaponName);
	const char *GetWeaponName(int WeaponIndex);	

private:
	int GetNextSpaceCount(const char *Text,int StartIndex,int Maxlen);

	unsigned long g_HashWeaponList[MAX_WEAPONS+1];
	char g_TextWeaponList[MAX_WEAPONS+1][MAX_WEAPONNAMELEN+1];
	int g_WeaponListCount;
};
#endif //PlayerUtils