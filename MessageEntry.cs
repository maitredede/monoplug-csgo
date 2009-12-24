
using System;

namespace MonoPlug
{
	internal struct MessageEntry
	{
		public MessageType Type;
		public string Message;
		
		public MessageEntry(MessageType type, string message)
		{
			this.Type = type;
			this.Message = message;
		}
	}
}
