#include "LoggingListener.h"
#include "Helpers.h"

LoggingListener::LoggingListener()
{
	this->pBuffer = NULL;
	this->iBufferLen = 0;
}
LoggingListener::~LoggingListener()
{
	while (this->pBuffer){
		Node* next = this->pBuffer->next;
		delete this->pBuffer->str;
		delete this->pBuffer;
		this->pBuffer = next;
	}
}

void LoggingListener::Log(const LoggingContext_t *pContext, const tchar *pMessage)
{
	size_t msgLen = strlen(pMessage);
	size_t buffLen = msgLen + 1;
	if (this->pBuffer){
		Node* last = this->pBuffer;
		while (last->next){
			last = last->next;
		}
		last->next = new Node();
		last->next->next = NULL;
		last->next->str = new char[buffLen];
		ZeroMemory(last->next->str, buffLen);
		strcpy_s(last->next->str, buffLen, pMessage);
	}
	else{
		this->pBuffer = new Node();
		this->pBuffer->next = NULL;
		this->pBuffer->str = new char[buffLen];
		ZeroMemory(this->pBuffer->str, buffLen);
		strcpy_s(this->pBuffer->str, buffLen, pMessage);
	}
	this->iBufferLen += msgLen;
}

void LoggingListener::Dump(char** dest, int* size)
{
	size_t bufLen = this->iBufferLen + 1;
	char* buffer = new char[bufLen];
	ZeroMemory(buffer, bufLen);

	Node* pNode = this->pBuffer;
	while (pNode){
		strcat_s(buffer, bufLen, pNode->str);
		pNode = pNode->next;
	}

	*dest = buffer;
	*size = this->iBufferLen;
}