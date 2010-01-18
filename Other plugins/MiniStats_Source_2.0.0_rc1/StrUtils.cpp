#include "StrUtils.h"

char* StrUtil::StrRemoveQuotes(char *text)
{
	int len = strlen(text);

	for(int i=0;i<len;i++)
	{
		if(text[i] == '\"')
			text[i] = NULL;
	}
	return text;
}

int StrUtil::GetFirstIndexOfChar(const char *Text,int MaxLen,unsigned char t)
{
	int TextIndex = 0;
	bool WSS = false; // WhiteSpaceSearch
	if(StrIsSpace(t))
		WSS = true;

	while(TextIndex <= MaxLen)
	{
		if(WSS)		// If the caller is searching for a whitespace, we make sure to search for the other whitespaces to.
		{
			if(StrIsSpace(Text[TextIndex]))
				return TextIndex;
			else
				TextIndex++;
		}
		else if(Text[TextIndex] == t)
		{
			return TextIndex;
		}
		else
			TextIndex++;
	}
	return -1;
}

int StrUtil::StrReplace(char *str, const char *from, const char *to, int maxlen) // Function from sslice 
{
	char  *pstr   = str;
	int   fromlen = Q_strlen(from);
	int   tolen   = Q_strlen(to);
	int	  RC=0;		// Removed count

	while (*pstr != '\0' && pstr - str < maxlen) {
		if (Q_strncmp(pstr, from, fromlen) != 0) {
			*pstr++;
			continue;
		}
		Q_memmove(pstr + tolen, pstr + fromlen, maxlen - ((pstr + tolen) - str) - 1);
		Q_memcpy(pstr, to, tolen);
		pstr += tolen;
		RC++;
	}
	return RC;
}

void StrUtil::StrTrim(char *buffer)
{
	StrTrimLeft(buffer);
	StrTrimRight(buffer);	
}

void StrUtil::StrTrimRight(char *buffer)
{
	// Make sure buffer isn't null
	if (buffer)
	{
		// Loop through buffer backwards while replacing whitespace chars with null chars
		for (int i = (int)strlen(buffer) - 1; i >= 0; i--)
		{
			if (StrIsSpace(buffer[i]))
				buffer[i] = '\0';
			else
				break;
		}
	}
}
bool StrUtil::StrIsSpace(unsigned char b)
{
	switch (b)
	{
	case ' ': 
		return true;
	case '\n':
		return true;
	case '\r':
		return true;
	case '\0': 
		return true;
	}
	return false;
}
/*
More info can be found here: Hashes
http://en.wikipedia.org/wiki/Duff's_device
*/
unsigned long StrUtil::StrHash(register const char *str, register int len) 
{
	register unsigned long n = 0;

#define HASHC	n = *str++ + 65587 * n  // The other number is better somehow, magic i guess ( Old num: 65599 )

	if (len > 0) {
		register int loop = (len + 8 - 1) >> 3;

		switch(len & (8 - 1)) {
		case 0:	do {
			HASHC;	case 7:	HASHC;
		case 6:	HASHC;	case 5:	HASHC;
		case 4:	HASHC;	case 3:	HASHC;
		case 2:	HASHC;	case 1:	HASHC;
				} while (--loop);
		}
	}
	return n;
}
void StrUtil::StrTrimLeft(char *buffer)
{
	// Let's think of this as our iterator
	char *i = buffer;

	// Make sure the buffer isn't null
	if (i && *i)
	{
		// Add up number of whitespace characters
		while(StrIsSpace(*i))
			i++;

		// If whitespace chars in buffer then adjust string so first non-whitespace char is at start of buffer
		// :TODO: change this to not use memcpy()!
		if (i != buffer)
			memcpy(buffer, i, (strlen(i) + 1) * sizeof(char));
	}
}