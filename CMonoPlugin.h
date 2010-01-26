#ifndef _CMONOPLUGIN_H_
#define _CMONOPLUGIN_H_

#include "monoBase.h"
#include "pluginterfaces.h"
#include "CMonoHelpers.h"
#include "CBaseAccessor.h"
#include "sourcehook.h"

namespace MonoPlugin
{
	class CMonoPlugin : public ISmmPlugin, public IMetamodListener
	{
	public:
		bool Load(PluginId id, ISmmAPI *ismm, char *error, size_t maxlen, bool late);
		bool Unload(char *error, size_t maxlen);
		bool Pause(char *error, size_t maxlen);
		bool Unpause(char *error, size_t maxlen);
		void AllPluginsLoaded();
	public:
		const char *GetAuthor();
		const char *GetName();
		const char *GetDescription();
		const char *GetURL();
		const char *GetLicense();
		const char *GetVersion();
		const char *GetDate();
		const char *GetLogTag();
	private:
		bool InitMono(const char* dllFile, char *error, size_t maxlen);
		bool StartMono(char *error, size_t maxlen);
		void AddHooks();
		void RemoveHooks();
	public:
		MonoAssembly* m_assembly;
		MonoImage* m_image;

		MonoClass* m_class_ClsMain;
		MonoMethod* m_ClsMain_Init;
		MonoMethod* m_ClsMain_GameFrame;

		MonoObject* m_main;

		ConVar* m_test;
	public: //IMetamodListener stuff
		void OnVSPListening(IServerPluginCallbacks *iface);
	public:
		void Hook_GameFrame(bool simulating);
		ICvar *GetICVar();
#if SOURCE_ENGINE >= SE_ORANGEBOX == 1
	int GetApiVersion() { return METAMOD_PLAPI_VERSION; }
#endif
	};

	//void Hook_ServerActivate(edict_t *pEdictList, int edictCount, int clientMax);
}

extern MonoPlugin::CMonoPlugin g_MonoPlugin;

PLUGIN_GLOBALVARS();

#if defined WIN32 || defined _WIN32
	#define MONOPLUG_DLLFILE "%s\\addons\\MonoPlug.Managed.dll"
#else
	#define MONOPLUG_DLLFILE "%s/addons/MonoPlug.Managed.dll"
#endif

#endif //_CMONOPLUGIN_H_
