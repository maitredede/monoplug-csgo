#ifndef _DOTNETPLUG_ADMIN_INTERFACE_H_
#define _DOTNETPLUG_ADMIN_INTERFACE_H_
#ifdef _WIN32
#pragma once
#endif

#define ADMININTERFACE_VERSION 1
#define ADMININTERFACE_MAXACCESSLENGTHTEXT 20		// This is the maximum length of a "flag" access text.

class AdminInterfaceListner
{
public:
	virtual void OnAdminInterfaceUnload() = 0;
	virtual void Client_Authorized(int id) = 0;
};

class AdminInterface
{
public:
	virtual bool RegisterFlag(const char *Class, const char *Flag, const char *Description) = 0; // Registers a new admin access
	virtual bool IsClient(int id) = 0;				// returns false if client is bot, or NOT connected
	virtual bool HasFlag(int id, const char *Class, const char *Flag) = 0;	// returns true if the player has this access flag, case sensitive
	virtual bool HasFlag(int id, const char *Flag) = 0;	// returns true if the player has this access flag, case sensitive, Assumes the class type of 'Admin'
	virtual int GetInterfaceVersion() = 0;		// Returns the interface version of the admin mod
	virtual const char* GetModName() = 0;			// Returns the name of the current admin mod
	virtual void AddEventListner(AdminInterfaceListner *ptr) = 0; // You should ALLWAYS set this, so you know when the "server"  plugin gets unloaded
	virtual void RemoveListner(AdminInterfaceListner *ptr) = 0;   // You MUST CALL this function in your plugin unloads function, or the admin plugin will crash on next client connect.
};

typedef struct
{
	AdminInterfaceListner *ptr;
} AdminInterfaceListnerStruct;

#endif //_DOTNETPLUG_ADMIN_INTERFACE_H_