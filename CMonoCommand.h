#ifndef _MONOCOMMAND_H_
#define _MONOCOMMAND_H_

#include "monoBase.h"
#include <convar.h>
#include "utlvector.h"
//#include "CMonoPlug.h"
//#include "CMonoHelpers.h"

namespace MonoPlugin
{
	class CMonoCommand : public ConCommand
	{
	public:
		CMonoCommand(char* name, char* description, MonoDelegate* code, int flags, MonoDelegate* complete = NULL);
		int AutoCompleteSuggest( const char *partial, CUtlVector< CUtlString > &commands );
		bool CanAutoComplete( void );
		void Dispatch( const CCommand &command );
	private:
		MonoDelegate* m_code;
		MonoDelegate* m_complete;
	};
}

#endif //_MONOCOMMAND_H_
