#include "CMonoPlugin.h"

namespace MonoPlugin
{
	bool CMonoPlugin::InitMono(const char* dllFile, char *error, size_t maxlen)
	{
		if(!g_Domain)
		{
			if(!CMonoHelpers::GetDomain(dllFile, g_Domain, error, maxlen)) return false;
		}

		if(!CMonoHelpers::GetAssembly(dllFile, g_Domain, this->m_assembly, error, maxlen)) return false;
		if(!CMonoHelpers::GetImage(dllFile, this->m_assembly, this->m_image, error, maxlen)) return false;
		if(!CMonoHelpers::GetClass(this->m_image, "MonoPlug", "ClsMain", this->m_class_ClsMain, error, maxlen)) return false;
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:Init()", this->m_ClsMain_Init, error, maxlen)) return false;
		if(!CMonoHelpers::GetMethod(this->m_image, "MonoPlug.ClsMain:GameFrame()", this->m_ClsMain_GameFrame, error, maxlen)) return false;

		return true;
	}

	bool CMonoPlugin::StartMono(char *error, size_t maxlen)
	{
		MonoObject* ret = CMonoHelpers::CallMethod(this->m_main, this->m_ClsMain_Init, NULL);
		if(ret)
		{
			bool bRet = *(bool*)mono_object_unbox(ret);
			return bRet;
		}
		else
		{
			return false;
		}
	}
}
