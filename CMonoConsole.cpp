#include "CMonoConsole.h"

CMonoConsole::CMonoConsole(CMonoPlug* plug)
{
	this->m_plug = plug;
}

void CMonoConsole::ColorPrint(const Color &clr, const char *pMessage)
{
	this->MonoPrint(true, false, clr.r(), clr.g(), clr.b(), clr.a(), pMessage);	
}

void CMonoConsole::Print(const char *pMessage)
{
	this->MonoPrint(false, false, 0, 0, 0, 0, pMessage);
}

void CMonoConsole::DPrint(const char *pMessage)
{
	this->MonoPrint(false, true, 0, 0, 0, 0, pMessage);
}

void CMonoConsole::MonoPrint(bool hasColor, bool debug, int r, int g, int b, int a, const char *msg)
{
	void* args[7];
	args[0] = &hasColor;
	args[1] = &debug;
	args[2] = &r;
	args[3] = &g;
	args[4] = &b;
	args[5] = &a;
	args[6] = CMonoHelpers::MONO_STRING(g_Domain, msg);
	CMonoHelpers::MONO_CALL(this->m_plug->m_main, this->m_plug->m_ClsMain_ConPrint, args);
}
