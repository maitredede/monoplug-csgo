#ifndef _MONOCONCOMMAND_H_
#define _MONOCONCOMMAND_H_

#include "monoplug_common.h"

class MonoConCommand : public ConCommand
{
public:
	MonoConCommand(char* name, char* description, MonoDelegate* code, int flags);
private:
	void Dispatch( const CCommand &command );
	MonoDelegate* m_code;
};

#endif //_MONOCONCOMMAND_H_
