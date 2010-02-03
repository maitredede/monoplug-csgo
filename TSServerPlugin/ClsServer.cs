using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using anyID = System.UInt16;
using uint64 = System.UInt64;
using System.Runtime.InteropServices;

namespace TSServerPlugin
{
    internal sealed class ClsServer : MarshalByRefObject, IDisposable
    {
        private readonly server_callback_struct _callbackStruct;
        private readonly INativeMethods _dll;

        public ClsServer(INativeMethods dll)
        {
            if (dll == null) throw new ArgumentNullException();
            this._dll = dll;

            this._callbackStruct = new server_callback_struct();
            this._callbackStruct.onClientConnected_delegate = new onClientConnected_type(this.Dll_ClientConnected);
            //this._callbackStruct.onClientDisconnected_delegate = new onClientDisconnected_type(callback.onClientDisconnected);
            //this._callbackStruct.onClientMoved_delegate = new onClientMoved_type(callback.onClientMoved);
            //this._callbackStruct.onChannelCreated_delegate = new onChannelCreated_type(callback.onChannelCreated);
            //this._callbackStruct.onChannelEdited_delegate = new onChannelEdited_type(callback.onChannelEdited);
            //this._callbackStruct.onChannelDeleted_delegate = new onChannelDeleted_type(callback.onChannelDeleted);
            //this._callbackStruct.onTextMessageEvent_delegate = new onTextMessageEvent_type(callback.onTextMessageEvent);
            this._callbackStruct.onUserLoggingMessageEvent_delegate = new onUserLoggingMessageEvent_type(this.Dll_UserLoggingMessageEvent);
            //this._callbackStruct.onClientStartTalkingEvent_delegate = new onClientStartTalkingEvent_type(callback.onClientStartTalkingEvent);
            //this._callbackStruct.onClientStopTalkingEvent_delegate = new onClientStopTalkingEvent_type(callback.onClientStopTalkingEvent);
            //this._callbackStruct.onAccountingErrorEvent_delegate = new onAccountingErrorEvent_type(callback.onAccountingErrorEvent);

            ERROR err = this._dll.ts3server_initServerLib(ref this._callbackStruct, LogTypes.LogType_USERLOGGING, null);
            if (err != ERROR.ok)
            {
                throw new InvalidProgramException(string.Format("Failed to initialize serverlib: {0}", err));
            }
        }

        #region Callbacks
        private void Dll_ClientConnected(uint64 serverID, anyID clientID, uint64 channelID, ref uint removeClientError)
        {
        }

        private void Dll_UserLoggingMessageEvent(string logmessage, int logLevel, string logChannel, uint64 logID, string logTime, string completeLogString)
        {
        }
        #endregion

        public void Dispose()
        {
            /* Shutdown server lib */
            ERROR err = this._dll.ts3server_destroyServerLib();
            if (err != ERROR.ok)
            {
                throw new InvalidProgramException(string.Format("Error destroying server lib: {0}", err));
            }
        }
    }
}
