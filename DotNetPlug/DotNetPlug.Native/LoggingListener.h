#ifndef _DOTNETPLUG_LOGGING_LISTENER_H_
#define _DOTNETPLUG_LOGGING_LISTENER_H_

#include <logging.h>

class LoggingListener :public ILoggingListener
{
public:
	LoggingListener();
	~LoggingListener();

	void Log(const LoggingContext_t *pContext, const tchar *pMessage);
	void Dump(char** dest, int* size);
private:
	struct Node {
		char* str;
		Node *next;
	};
	Node* pBuffer;
	size_t iBufferLen;
	void Append(const char* pMessage);
};

#endif //_DOTNETPLUG_LOGGING_LISTENER_H_