#include "CMonoPlugin.h"

using namespace MonoPlugin;

//Plugin singleton
CMonoPlugin g_MonoPlugin;

PLUGIN_EXPOSE(CMonoPlugin, g_MonoPlugin);

//Basic plugin functions
bool MonoPlugin::CMonoPlugin::Pause(char *error, size_t maxlen)
{
	return true;
}

bool MonoPlugin::CMonoPlugin::Unpause(char *error, size_t maxlen)
{
	return true;
}

const char *MonoPlugin::CMonoPlugin::GetLicense()
{
	return "Licence not known yet";
}

const char *MonoPlugin::CMonoPlugin::GetVersion()
{
	return "0.2.0.0";
}

const char *MonoPlugin::CMonoPlugin::GetDate()
{
	return __DATE__;
}

const char *MonoPlugin::CMonoPlugin::GetLogTag()
{
	return "MONO";
}

const char *MonoPlugin::CMonoPlugin::GetAuthor()
{
	return "MaitreDede";
}

const char *MonoPlugin::CMonoPlugin::GetDescription()
{
	return "Mono plugin engine for Dotnet plugins";
}

const char *MonoPlugin::CMonoPlugin::GetName()
{
	return "MonoPlugin";
}

const char *MonoPlugin::CMonoPlugin::GetURL()
{
	return "http://www.sourcemm.net/";
}
