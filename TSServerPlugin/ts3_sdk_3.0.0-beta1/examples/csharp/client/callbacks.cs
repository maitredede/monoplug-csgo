/*
 * TeamSpeak 3 client minimal sample C#
 *
 * Copyright (c) 2007-2009 TeamSpeak-Systems
 */

using System;
using ts3client_minimal_sample;
using System.Runtime.InteropServices;
using anyID = System.UInt32;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onConnectStatusChangeEvent_type(anyID serverConnectionHandlerID, int newStatus, uint errorNumber);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onNewChannelEvent_type(anyID serverConnectionHandlerID, anyID channelID, anyID channelParentID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onNewChannelCreatedEvent_type(anyID serverConnectionHandlerID, anyID channelID, anyID channelParentID, anyID invokerID, string invokerName, string invokerUniqueIdentifier);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onDelChannelEvent_type(anyID serverConnectionHandlerID, anyID channelID, anyID invokerID, string invokerName, string invokerUniqueIdentifier);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelMoveEvent_type(anyID serverConnectionHandlerID, anyID channelID, anyID newChannelParentID, anyID invokerID, string invokerName, string invokerUniqueIdentifier);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onUpdateChannelEvent_type(anyID serverConnectionHandlerID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onUpdateChannelEditedEvent_type(anyID serverConnectionHandlerID, anyID channelID, anyID invokerID, string invokerName, string invokerUniqueIdentifier);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onUpdateClientEvent_type(anyID serverConnectionHandlerID, anyID clientID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientMoveEvent_type(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility, string moveMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientMoveSubscriptionEvent_type(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientMoveTimeoutEvent_type(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility, string timeoutMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientMoveMovedEvent_type(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility, anyID moverID, string moverName, string moverUniqueIdentifier, string moveMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientKickFromChannelEvent_type(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility, anyID kickerID, string kickerName, string kickerUniqueIdentifier, string kickMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientKickFromServerEvent_type(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility, anyID kickerID, string kickerName, string kickerUniqueIdentifier, string kickMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientIDsEvent_type(anyID serverConnectionHandlerID, string uniqueClientIdentifier, anyID clientID, string clientName);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientIDsFinishedEvent_type(anyID serverConnectionHandlerID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onServerEditedEvent_type(anyID serverConnectionHandlerID, anyID editerID, string editerName, string editerUniqueIdentifier);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onServerUpdatedEvent_type(anyID serverConnectionHandlerID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onServerErrorEvent_type(anyID serverConnectionHandlerID, string errorMessage, uint error, string returnCode, string extraMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onServerStopEvent_type(anyID serverConnectionHandlerID, string shutdownMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onTextMessageEvent_type(anyID serverConnectionHandlerID, anyID targetMode, anyID fromID, string fromName, string fromUniqueIdentifier, string message, ref anyID targets);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onTalkStatusChangeEvent_type(anyID serverConnectionHandlerID, int status, anyID clientID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onConnectionInfoEvent_type(anyID serverConnectionHandlerID, anyID clientID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onServerConnectionInfoEvent_type(anyID serverConnectionHandlerID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelSubscribeEvent_type(anyID serverConnectionHandlerID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelSubscribeFinishedEvent_type(anyID serverConnectionHandlerID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelUnsubscribeEvent_type(anyID serverConnectionHandlerID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelUnsubscribeFinishedEvent_type(anyID serverConnectionHandlerID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelDescriptionUpdateEvent_type(anyID serverConnectionHandlerID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelPasswordChangedEvent_type(anyID serverConnectionHandlerID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onCustomCaptureDeviceCloseEvent_type(anyID serverConnectionHandlerID, IntPtr/*void* */ fmodSystem);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onCustomPlaybackDeviceCloseEvent_type(anyID serverConnectionHandlerID, IntPtr/*void* */ fmodSystem);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onFMODChannelCreatedEvent_type(anyID serverConnectionHandlerID, anyID clientID, IntPtr/*void**/ fmodChannel);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onPlaybackShutdownCompleteEvent_type(anyID serverConnectionHandlerID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onUserLoggingMessageEvent_type(string logmessage, int logLevel, string logChannel, anyID logID, string logTime, string completeLogString);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onVoiceRecordDataEvent_type(float[] data, uint dataSize);

/* for rare, but unused in SDK */
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void dummy1_type();

/* for rare, but unused in SDK */
[StructLayout(LayoutKind.Sequential)]
public struct client_callbackrare_struct {
	public dummy1_type dummy1_delegate;
	public dummy1_type dummy2_delegate;
	public dummy1_type dummy3_delegate;
	public dummy1_type dummy4_delegate;
	public dummy1_type dummy5_delegate;
	public dummy1_type dummy6_delegate;
	public dummy1_type dummy7_delegate;
	public dummy1_type dummy8_delegate;
	public dummy1_type dummy9_delegate;
	public dummy1_type dummy10_delegate;
	public dummy1_type dummy11_delegate;
	public dummy1_type dummy12_delegate;
	public dummy1_type dummy13_delegate;
	public dummy1_type dummy14_delegate;
	public dummy1_type dummy15_delegate;
	public dummy1_type dummy16_delegate;
	public dummy1_type dummy17_delegate;
	public dummy1_type dummy18_delegate;
	public dummy1_type dummy19_delegate;
	public dummy1_type dummy20_delegate;
	public dummy1_type dummy21_delegate;
	public dummy1_type dummy22_delegate;
	public dummy1_type dummy23_delegate;
	public dummy1_type dummy24_delegate;
	public dummy1_type dummy25_delegate;
	public dummy1_type dummy26_delegate;
	public dummy1_type dummy27_delegate;
}

[StructLayout(LayoutKind.Sequential)]
public struct client_callback_struct {
	public onConnectStatusChangeEvent_type onConnectStatusChangeEvent_delegate;
	public onNewChannelEvent_type onNewChannelEvent_delegate;
	public onNewChannelCreatedEvent_type onNewChannelCreatedEvent_delegate;
	public onDelChannelEvent_type onDelChannelEvent_delegate;
	public onChannelMoveEvent_type onChannelMoveEvent_delegate;
	public onUpdateChannelEvent_type onUpdateChannelEvent_delegate;
	public onUpdateChannelEditedEvent_type onUpdateChannelEditedEvent_delegate;
	public onUpdateClientEvent_type onUpdateClientEvent_delegate;
	public onClientMoveEvent_type onClientMoveEvent_delegate;
	public onClientMoveSubscriptionEvent_type onClientMoveSubscriptionEvent_delegate;
	public onClientMoveTimeoutEvent_type onClientMoveTimeoutEvent_delegate;
	public onClientMoveMovedEvent_type onClientMoveMovedEvent_delegate;
	public onClientKickFromChannelEvent_type onClientKickFromChannelEvent_delegate;
	public onClientKickFromServerEvent_type onClientKickFromServerEvent_delegate;
	public onClientIDsEvent_type onClientIDsEvent_delegate;
	public onClientIDsFinishedEvent_type onClientIDsFinishedEvent_delegate;
	public onServerEditedEvent_type onServerEditedEvent_delegate;
	public onServerUpdatedEvent_type onServerUpdatedEvent_delegate;
	public onServerErrorEvent_type onServerErrorEvent_delegate;
	public onServerStopEvent_type onServerStopEvent_delegate;
	public onTextMessageEvent_type onTextMessageEvent_delegate;
	public onTalkStatusChangeEvent_type onTalkStatusChangeEvent_delegate;
	public onConnectionInfoEvent_type onConnectionInfoEvent_delegate;
    public onServerConnectionInfoEvent_type onServerConnectionInfoEvent_delegate;
	public onChannelSubscribeEvent_type onChannelSubscribeEvent_delegate;
	public onChannelSubscribeFinishedEvent_type onChannelSubscribeFinishedEvent_delegate;
	public onChannelUnsubscribeEvent_type onChannelUnsubscribeEvent_delegate;
	public onChannelUnsubscribeFinishedEvent_type onChannelUnsubscribeFinishedEvent_delegate;
	public onChannelDescriptionUpdateEvent_type onChannelDescriptionUpdateEvent_delegate;
	public onChannelPasswordChangedEvent_type onChannelPasswordChangedEvent_delegate;
	public onCustomCaptureDeviceCloseEvent_type onCustomCaptureDeviceCloseEvent_delegate;
	public onCustomPlaybackDeviceCloseEvent_type onCustomPlaybackDeviceCloseEvent_delegate;
	public onFMODChannelCreatedEvent_type onFMODChannelCreatedEvent_delegate;
	public onPlaybackShutdownCompleteEvent_type onPlaybackShutdownCompleteEvent_delegate;
	public onUserLoggingMessageEvent_type onUserLoggingMessageEvent_delegate;
	public onVoiceRecordDataEvent_type onVoiceRecordDataEvent_delegate;
    public dummy1_type dummy1_delegate;
    public dummy1_type dummy2_delegate;
}

public static class callback {
	/*
	* Callback for connection status change.
	* Connection status switches through the states STATUS_DISCONNECTED, STATUS_CONNECTING, STATUS_CONNECTED and STATUS_CONNECTION_ESTABLISHED.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   newStatus                 - New connection status, see the enum ConnectStatus in clientlib_publicdefinitions.h
	*   errorNumber               - Error code. Should be zero when connecting or actively disconnection.
	*                               Contains error state when losing connection.
	*/
	public static void onConnectStatusChangeEvent(anyID serverConnectionHandlerID, int newStatus, uint errorNumber) {
		Console.WriteLine("Connect status changed: {0} {1} {2}", serverConnectionHandlerID, newStatus, errorNumber);
		/* Failed to connect ? */
		if (newStatus == (int)ConnectStatus.STATUS_DISCONNECTED && errorNumber == public_errors.ERROR_failed_connection_initialisation) {
			Console.WriteLine("Looks like there is no server running, terminate!\n");
			Console.ReadLine();
			System.Environment.Exit(-1);
		}
	}

	/*
	* Callback for current channels being announced to the client after connecting to a server.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   channelID                 - ID of the announced channel
	*   channelParentID           - ID of the parent channel
	*/
	public static void onNewChannelEvent(anyID serverConnectionHandlerID, anyID channelID, anyID channelParentID) {
		string name;
		string errormsg;
		uint error;
		IntPtr namePtr = IntPtr.Zero;
		Console.WriteLine("onNewChannelEvent: {0} {1} {2}", serverConnectionHandlerID, channelID, channelParentID);
		error = ts3client.ts3client_getChannelVariableAsString(serverConnectionHandlerID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr);
		if (error == public_errors.ERROR_ok) {
			name = Marshal.PtrToStringAnsi(namePtr);
			Console.WriteLine("New channel: {0} {1}", channelID, name);
			ts3client.ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
		} else {
			IntPtr errormsgPtr = IntPtr.Zero;
			if (ts3client.ts3client_getErrorMessage(error, errormsgPtr) == public_errors.ERROR_ok) {
				errormsg = Marshal.PtrToStringAnsi(errormsgPtr);
				Console.WriteLine("Error getting channel name in onNewChannelEvent: {0}", errormsg);
				ts3client.ts3client_freeMemory(errormsgPtr);
			}
		}
	}

	/*
	* Callback for just created channels.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   channelID                 - ID of the announced channel
	*   channelParentID           - ID of the parent channel
	*   invokerID                 - ID of the client who created the channel
	*   invokerName               - Name of the client who created the channel
	*/
	public static void onNewChannelCreatedEvent(anyID serverConnectionHandlerID, anyID channelID, anyID channelParentID, anyID invokerID, string invokerName, string invokerUniqueIdentifier) {
		string name;
		IntPtr namePtr = IntPtr.Zero;
		/* Query channel name from channel ID */
		uint error = ts3client.ts3client_getChannelVariableAsString(serverConnectionHandlerID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr);
		if (error != public_errors.ERROR_ok) {
			return;
		}
		name = Marshal.PtrToStringAnsi(namePtr);
		Console.WriteLine("New channel created: {0}", name);
		ts3client.ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
	}

	/*
	* Callback when a channel was deleted.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   channelID                 - ID of the deleted channel
	*   invokerID                 - ID of the client who deleted the channel
	*   invokerName               - Name of the client who deleted the channel
	*/
	public static void onDelChannelEvent(anyID serverConnectionHandlerID, anyID channelID, anyID invokerID, string invokerName, string invokerUniqueIdentifier) {
		Console.WriteLine("Channel ID {0} deleted by {1} ({2})", channelID, invokerName, invokerID);
	}

	/*
	* Called when a client joins, leaves or moves to another channel.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   clientID                  - ID of the moved client
	*   oldChannelID              - ID of the old channel left by the client
	*   newChannelID              - ID of the new channel joined by the client
	*   visibility                - Visibility of the moved client. See the enum Visibility in clientlib_publicdefinitions.h
	*                               Values: ENTER_VISIBILITY, RETAIN_VISIBILITY, LEAVE_VISIBILITY
	*/
	public static void onClientMoveEvent(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility, string moveMessage) {
		Console.WriteLine("ClientID {0} moves from channel {1} to {2} with message {3}", clientID, oldChannelID, newChannelID, moveMessage);
	}

	/*
	* Callback for other clients in current and subscribed channels being announced to the client.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   clientID                  - ID of the announced client
	*   oldChannelID              - ID of the subscribed channel where the client left visibility
	*   newChannelID              - ID of the subscribed channel where the client entered visibility
	*   visibility                - Visibility of the announced client. See the enum Visibility in clientlib_publicdefinitions.h
	*                               Values: ENTER_VISIBILITY, RETAIN_VISIBILITY, LEAVE_VISIBILITY
	*/
	public static void onClientMoveSubscriptionEvent(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility) {
		string name;
		IntPtr namePtr = IntPtr.Zero;
		/* Query client nickname from ID */
		uint error = ts3client.ts3client_getClientVariableAsString(serverConnectionHandlerID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr);
		if (error != public_errors.ERROR_ok) {
			return;
		}
		name = Marshal.PtrToStringAnsi(namePtr);
		Console.WriteLine("New client: {0}", name);
		ts3client.ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
	}

	/*
	* Called when a client drops his connection.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   clientID                  - ID of the moved client
	*   oldChannelID              - ID of the channel the leaving client was previously member of
	*   newChannelID              - 0, as client is leaving
	*   visibility                - Always LEAVE_VISIBILITY
	*   timeoutMessage            - Optional message giving the reason for the timeout
	*/
	public static void onClientMoveTimeoutEvent(anyID serverConnectionHandlerID, anyID clientID, anyID oldChannelID, anyID newChannelID, int visibility, string timeoutMessage) {
		Console.WriteLine("ClientID {0} timeouts with message {1}", clientID, timeoutMessage);
	}

	/*
	* This event is called when a client starts or stops talking.
	*
	* Parameters:
	*   serverConnectionHandlerID - Server connection handler ID
	*   status                    - 1 if client starts talking, 0 if client stops talking
	*   clientID                  - ID of the client who announced the talk status change
	*/
	public static void onTalkStatusChangeEvent(anyID serverConnectionHandlerID, int status, anyID clientID) {
		string name;
		IntPtr namePtr = IntPtr.Zero;
		/* Query client nickname from ID */
		uint error = ts3client.ts3client_getClientVariableAsString(serverConnectionHandlerID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr);
		if (error != public_errors.ERROR_ok) {
			return;
		}
		name = Marshal.PtrToStringAnsi(namePtr);
		if (status == (int)TalkStatus.STATUS_TALKING) {
			Console.WriteLine("Client \"{0}\" starts talking.\n", name);
		} else {
			Console.WriteLine("Client \"{0}\" stops talking.\n", name);
		}
		ts3client.ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
	}

	public static void onServerErrorEvent(anyID serverConnectionHandlerID, string errorMessage, uint error, string returnCode, string extraMessage) {
		Console.WriteLine("Error for server {0}: {1}\n", serverConnectionHandlerID, errorMessage);
	}

    public static void onServerStopEvent(anyID serverConnectionHandlerID, string shutdownMessage) {
        Console.WriteLine("Server {0} stopping: {1}", serverConnectionHandlerID, shutdownMessage);
    }
}
