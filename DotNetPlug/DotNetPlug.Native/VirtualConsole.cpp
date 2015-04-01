#include "VirtualConsole.h"
#include "Helpers.h"

VirtualConsole::VirtualConsole(){
	this->pBuffer = NULL;
	this->iBufferLen = 0;
}

VirtualConsole::~VirtualConsole(){
	while (this->pBuffer){
		Node* next = this->pBuffer->next;
		delete this->pBuffer->str;
		delete this->pBuffer;
		this->pBuffer = next;
	}
}

void VirtualConsole::Append(const char* msg)
{
	if (!msg)
		return;

	size_t msgLen = strlen(msg);
	if (this->pBuffer){
		Node* last = this->pBuffer;
		while (last->next){
			last = last->next;
		}
		last->next = new Node();
		last->next->next = NULL;
		last->next->str = new char[msgLen + 1];
		ZeroMemory(last->next->str, msgLen + 1);
		strcpy_s(last->next->str, msgLen, msg);
	}
	else{
		this->pBuffer = new Node();
		this->pBuffer->next = NULL;
		this->pBuffer->str = new char[msgLen + 1];
		ZeroMemory(this->pBuffer->str, msgLen + 1);
		strcpy_s(this->pBuffer->str, msgLen, msg);
	}
	this->iBufferLen += msgLen;
}

void VirtualConsole::ColorPrint(const Color& clr, const char *pMessage)
{
	this->Append(pMessage);
}
void VirtualConsole::Print(const char *pMessage)
{
	this->Append(pMessage);
}
void VirtualConsole::DPrint(const char *pMessage)
{
	//Nothing to do here
}

void VirtualConsole::GetConsoleText(char *pchText, size_t bufSize) const
{
	//TODO ?
}

void VirtualConsole::Dump(char** dest, size_t* size)
{
	char* buffer = new char[this->iBufferLen + 1];
	ZeroMemory(buffer, this->iBufferLen + 1);

	Node* pNode = this->pBuffer;
	while (pNode){
		strcat_s(buffer, strlen(pNode->str), pNode->str);
		pNode = pNode->next;
	}

	*dest = buffer;
	*size = this->iBufferLen;
}