#ifndef _CMONOCONSOLE_H_
#define _CMONOCONSOLE_H_

#include "monoBase.h"
#include <icvar.h>
#include "CMonoPlugin.h"

namespace MonoPlugin
{
	class CMonoPlugin;

	class CMonoConsole : public IConsoleDisplayFunc
	{
	private:
		CMonoPlugin* m_plug;
		void MonoPrint(bool hasColor, bool debug, int r, int g, int b, int a, const char * msg);
	public:
		CMonoConsole(CMonoPlugin* plug);
		virtual void ColorPrint( const Color& clr, const char *pMessage );
		virtual void Print( const char *pMessage );
		virtual void DPrint( const char *pMessage );
	};
}

#endif //_CMONOCONSOLE_H_
