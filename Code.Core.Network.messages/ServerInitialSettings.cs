using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages;

public class ServerInitialSettings : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 420;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInitialSettings(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 420;

	private readonly List<string> _events;

	public List<string> Events => _events;

	private ServerInitialSettings(MessageReader reader)
	{
		_events = new List<string>();
		short num = reader.ReadInt16();
		for (int i = 0; i < num; i++)
		{
			_events.Add(reader.ReadString());
		}
	}

	public byte[] GetData()
	{
		int num = 6;
		num += (short)(2 * _events.Count);
		foreach (string @event in _events)
		{
			num += MessageWriter.GetSize(@event);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(420);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)_events.Count);
		foreach (string event2 in _events)
		{
			messageWriter.WriteString(event2);
		}
		return messageWriter.GetData();
	}
}
