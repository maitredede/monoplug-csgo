#include "monoBase.h"
#include "pluginterfaces.h"

IServerGameDLL *g_iserver = NULL;
IFileSystem *g_filesystem = NULL;
IVEngineServer *g_engine = NULL;

MonoDomain *g_Domain = NULL;

CGlobalVars *gpGlobals = NULL;
ICvar *icvar = NULL;
IServerPluginCallbacks* g_vsp_callbacks = NULL;
IServerPluginHelpers* g_helpers = NULL;
