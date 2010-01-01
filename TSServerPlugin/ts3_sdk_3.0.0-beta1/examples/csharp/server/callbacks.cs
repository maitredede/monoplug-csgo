/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2009 TeamSpeak-Systems
 */

using System;
using ts3_server_minimal_sample;
using System.Runtime.InteropServices;
using anyID = System.UInt32;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onVoiceDataEvent_type(anyID serverID, anyID clientID, string voiceData, uint voiceDataSize, uint frequency);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientStartTalkingEvent_type(anyID serverID, anyID clientID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientStopTalkingEvent_type(anyID serverID, anyID clientID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientConnected_type(anyID serverID, anyID clientID, anyID channelID, ref uint removeClientError);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientDisconnected_type(anyID serverID, anyID clientID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientMoved_type(anyID serverID, anyID clientID, anyID oldChannelID, anyID newChannelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelCreated_type(anyID serverID, anyID invokerClientID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelEdited_type(anyID serverID, anyID invokerClientID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelDeleted_type(anyID serverID, anyID invokerClientID, anyID channelID);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onTextMessageEvent_type(anyID serverID, int textMessageMode, anyID invokerClientID, ref anyID targetIDS, string textMessage);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onUserLoggingMessageEvent_type(string logmessage, int logLevel, string logChannel, anyID logID, string logTime, string completeLogString);
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onAccountingErrorEvent_type(anyID serverID, int errorCode);

/* For unused callbacks */
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void dummy_type();

[StructLayout(LayoutKind.Sequential)]
public struct server_callback_struct {
	public onVoiceDataEvent_type onVoiceDataEvent_delegate;
	public onClientStartTalkingEvent_type onClientStartTalkingEvent_delegate;
	public onClientStopTalkingEvent_type onClientStopTalkingEvent_delegate;
	public onClientConnected_type onClientConnected_delegate;
	public onClientDisconnected_type onClientDisconnected_delegate;
    public onClientMoved_type onClientMoved_delegate;
	public onChannelCreated_type onChannelCreated_delegate;
	public onChannelEdited_type onChannelEdited_delegate;
	public onChannelDeleted_type onChannelDeleted_delegate;
    public onTextMessageEvent_type onTextMessageEvent_delegate;
	public onUserLoggingMessageEvent_type onUserLoggingMessageEvent_delegate;
    public onAccountingErrorEvent_type onAccountingErrorEvent_delegate;
    public dummy_type dummy1_delegate;  // onCustomPacketEncryptEvent unused
    public dummy_type dummy2_delegate;  // onCustomPacketDecryptEvent unused
}

public static class callback {
	/*
	 * Callback when client has connected.
	 *
	 * Parameter:
	 *   serverID  - Virtual server ID
	 *   clientID  - ID of connected client
	 *   channelID - ID of channel the client joined
	 */
	public static void onClientConnected(anyID serverID, anyID clientID, anyID channelID, ref uint removeClientError) {
		Console.WriteLine("Client {0} joined channel {1} on virtual server {2}", clientID, channelID, serverID);
	}

	/*
	 * Callback when client has disconnected.
	 *
	 * Parameter:
	 *   serverID  - Virtual server ID
	 *   clientID  - ID of disconnected client
	 *   channelID - ID of channel the client left
	 */
	public static void onClientDisconnected(anyID serverID, anyID clientID, anyID channelID) {
		Console.WriteLine("Client {0} left channel {1} on virtual server {2}", clientID, channelID, serverID);
	}

    /*
     * Callback when client has moved.
     *
     * Parameter:
     *   serverID     - Virtual server ID
     *   clientID     - ID of moved client
     *   oldChannelID - ID of old channel the client left
     *   newChannelID - ID of new channel the client joined
     */
    public static void onClientMoved(anyID serverID, anyID clientID, anyID oldChannelID, anyID newChannelID)
    {
        Console.WriteLine("Client {0} moved from channel {1} to channel {2} on virtual server {3}\n", clientID, oldChannelID, newChannelID, serverID);
    }

	/*
	 * Callback when channel has been created.
	 *
	 * Parameter:
	 *   serverID        - Virtual server ID
	 *   invokerClientID - ID of the client who created the channel
	 *   channelID       - ID of the created channel
	 */
	public static void onChannelCreated(anyID serverID, anyID invokerClientID, anyID channelID) {
		Console.WriteLine("Channel {0} created by {1} on virtual server {2}", channelID, invokerClientID, serverID);
	}

	/*
	 * Callback when channel has been edited.
	 *
	 * Parameter:
	 *   serverID        - Virtual server ID
	 *   invokerClientID - ID of the client who edited the channel
	 *   channelID       - ID of the edited channel
	 */
	public static void onChannelEdited(anyID serverID, anyID invokerClientID, anyID channelID) {
		Console.WriteLine("Channel {0} edited by {1} on virtual server {2}", channelID, invokerClientID, serverID);
	}

	/*
	 * Callback when channel has been deleted.
	 *
	 * Parameter:
	 *   serverID        - Virtual server ID
	 *   invokerClientID - ID of the client who deleted the channel
	 *   channelID       - ID of the deleted channel
	 */
	public static void onChannelDeleted(anyID serverID, anyID invokerClientID, anyID channelID) {
		Console.WriteLine("Channel {0} deleted by {1} on virtual server {2}", channelID, invokerClientID, serverID);
	}

    /*
     * Callback when a text message has been received.
     * Note that only server and channel chats are received, private client are not caught due to privacy reasons.
     *
     * Parameter:
     *   serverID        - Virtual server ID
     *   textMessageMode - Indicates if the receiver is a channel, server or client. See enum TextMessageTargetMode 
     *   invokerClientID - ID of the client who sent the text message.
     *   targetIDS       - Target IDs
     *   textMessage     - Message text
     */
    public static void onTextMessageEvent(anyID serverID, int textMessageMode, anyID invokerClientID, ref anyID targetIDS, string textMessage) {
        // Note: Client targets are not received
        if(textMessageMode == 2) {  // Channel
            Console.WriteLine("Text message in channel {0} by {1}: {2}", targetIDS, invokerClientID, textMessage);
        } else if(textMessageMode == 3) {  // Server
            Console.WriteLine("Text message in server chat by {0}: {1}", invokerClientID, textMessage);
        }
    }

	/*
	 * Callback for user-defined logging.
	 *
	 * Parameter:
	 *   logMessage        - Log message text
	 *   logLevel          - Severity of log message
	 *   logChannel        - Custom text to categorize the message channel
	 *   logID             - Virtual server ID giving the virtual server source of the log event
	 *   logTime           - String with the date and time the log entry occured
	 *   completeLogString - Verbose log message including all previous parameters for convinience
	 */
	public static void onUserLoggingMessageEvent(string logMessage, int logLevel, string logChannel, anyID logID, string logTime, string completeLogString) {
	/* Your custom error display here... */
		/* Console.WriteLine("LOG: {0}", completeLogString); */
		if (logLevel == (int)LogLevel.LogLevel_CRITICAL) {
			System.Environment.Exit(1);  /* Your custom handling of critical errors */
		}
	}

	/*
	 * Callback triggered when the specified client starts talking.
	 *
	 * Parameters:
	 *   serverID - ID of the server sending the callback
	 *   clientID - ID of the client which started talking
	 */
	public static void onClientStartTalkingEvent(anyID serverID, anyID clientID) {
		Console.WriteLine("onClientStartTalkingEvent serverID={0}, clientID={1}", serverID, clientID);
	}

	/*
	 * Callback triggered when the specified client stops talking.
	 *
	 * Parameters:
	 *   serverID - ID of the server sending the callback
	 *   clientID - ID of the client which stopped talking
	 */
	public static void onClientStopTalkingEvent(anyID serverID, anyID clientID) {
		Console.WriteLine("onClientStopTalkingEvent serverID={0}, clientID={1}", serverID, clientID);
	}

    /*
     * Callback triggered when a license error occurs.
     *
     * Parameters:
     *   serverID  - ID of the virtual server on which the license error occured. This virtual server will be automatically
     *               shutdown. If the parameter is zero, all virtual servers are affected and have been shutdown.
     *   errorCode - Code of the occured error. Use ts3server_getGlobalErrorMessage() to convert to a message string
     */
    public static void onAccountingErrorEvent(anyID serverID, int errorCode) {
        Console.WriteLine("onAccountingErrorEvent serverID={0}, errorCode={1}", serverID, errorCode);
        System.Environment.Exit(1);
    }
}
