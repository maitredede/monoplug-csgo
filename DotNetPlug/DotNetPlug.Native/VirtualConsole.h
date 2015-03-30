#ifndef _DOTNETPLUG_VIRTUALCONSOLE_H_
#define _DOTNETPLUG_VIRTUALCONSOLE_H_

#include <eiface.h>
#include <icvar.h>

class VirtualConsole :IConsoleDisplayFunc{
public:
	void ColorPrint(const Color& clr, const char *pMessage);
	void Print(const char *pMessage);
	void DPrint(const char *pMessage);
	void GetConsoleText(char *pchText, size_t bufSize);

	VirtualConsole();
	~VirtualConsole();
	void Dump(char** dest, size_t* size);
private:
	struct Node {
		char* str;
		Node *next;
	};
	Node* pBuffer;
	size_t iBufferLen;
	void Append(const char* pMessage);
};

#endif // _DOTNETPLUG_VIRTUALCONSOLE_H_