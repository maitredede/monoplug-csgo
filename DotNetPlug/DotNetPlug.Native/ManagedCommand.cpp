#include "ManagedCommand.h"

ManagedCommand::ManagedCommand(int id, const char* cmd, const char* description, int flags, MANAGED_COMMAND_CALLBACK managedCallback)
{
	this->m_id = id;
	this->m_managedCallback = managedCallback;

	FnCommandCallback_t nativeCallback = &ManagedCommand::NativeCallback;

	this->m_nativeCommand = new ConCommand(cmd, nativeCallback, description, flags);
}

void ManagedCommand::NativeCallback(const CCommand &command){
	const char* cmdChar = command.ArgV()[0];
	ManagedCommand* cmd = g_Managed.GetCommand(cmdChar);
	if (cmd){
		cmd->InvokeCallback(command.ArgC(), command.ArgV());
	}
	else{
		//TODO : Error handling
	}
}

ConCommand* ManagedCommand::GetNativeCommand()
{
	return this->m_nativeCommand;
}


#ifdef MANAGED_WIN32
void ManagedCommand::InvokeCallback(int argc, const char** argv)
{
	//HRESULT hr;
	//variant_t vtArgs;

	//hr = CREATE_STRING_ARRAY(argc, argv, &vtArgs);
	//if (FAILED(hr)){
	//	META_CONPRINTF("Failed to create string array w/hr 0x%08lx\n", hr);
	//	return;
	//}
	//this->m_managedCallback(vtArgs.parray);

	//////////////////////////////////////////////////////////////////////////////
	HRESULT hr;
	SAFEARRAY *args = NULL;
	/*hr = CREATE_STRING_ARRAY_ARGS(argc, argv, &args);
	if (FAILED(hr))
	{
	META_CONPRINTF("Failed to create string array w/hr 0x%08lx\n", hr);
	hr = SafeArrayDestroy(args);
	if (FAILED(hr))
	{
	META_CONPRINTF("Failed to destroy array w/hr 0x%08lx\n", hr);
	}
	return;
	}
	variant_t param;
	LONG index = 0;
	hr = SafeArrayGetElement(args, &index, &param);
	if (FAILED(hr))
	{
	META_CONPRINTF("Failed to destroy array w/hr 0x%08lx\n", hr);
	}*/

	args = SafeArrayCreateVector(VT_BSTR, 0, argc);
	for (long i = 0; i < argc; i++){
		VARIANT item;
		VariantInit(&item);
		item.vt = VT_BSTR;
		item.bstrVal = _com_util::ConvertStringToBSTR(argv[i]);
		hr = SafeArrayPutElement(args, &i, &item);
		SysFreeString(item.bstrVal);
		if (FAILED(hr))
		{
			META_LOG(g_PLAPI, "Failed to set array item w/hr 0x%08lx\n", hr);
			hr = SafeArrayDestroy(args);
			if (FAILED(hr))
			{
				META_CONPRINTF("Failed to destroy array w/hr 0x%08lx\n", hr);
			}
			return;
		}
	}

	variant_t vtOutput = NULL;
	this->m_managedCallback(argc, args);
	hr = SafeArrayDestroy(args);
	if (FAILED(hr))
	{
		META_CONPRINTF("Failed to destroy array w/hr 0x%08lx\n", hr);
	}

	//hr = this->spPluginManagerLoadAssembly->Invoke_3(this->vtPluginManager, args, &vtOutput);
	//if (FAILED(hr))
	//{
	//	META_CONPRINTF("Failed to call PluginManager.LoadAssembly w/hr 0x%08lx\n", hr);
	//}

	/*hr = SafeArrayDestroy(args);
	if (FAILED(hr))
	{
	META_CONPRINTF("Failed to destroy array w/hr 0x%08lx\n", hr);
	}*/
}
#endif
#ifdef MANAGED_MONO
void ManagedCommand::InvokeCallback(int argc, const char** argv){
#error TODO
}
#endif