#ifndef _INCLUDE_StrUtil_CLASS
#define _INCLUDE_StrUtil_CLASS

#include <string.h>
#include <strtools.h>

class StrUtil
{
public:
	int StrReplace(char *str, const char *from, const char *to, int maxlen); // Replaces part of a string with something else
	char* StrRemoveQuotes(char *text);			// Removes Quotes from the string
	int GetFirstIndexOfChar(const char *Text,int MaxLen,unsigned char t);	// Gets index of the instance of t in a char array
	void StrTrimLeft(char *buffer);	// Trim from the left ( Removes white spaces )
	void StrTrimRight(char *buffer); // Trim from the right ( Removes white spaces )
	void StrTrim(char *buffer);		// Trims the string ( Removes white spaces )
	bool StrIsSpace(unsigned char b);	// If the char is a space or whitespace
	unsigned long StrHash(register const char *str, register int len); // Hashes the string  ( Uses dbm hash ( Duff's device version )
};

#endif