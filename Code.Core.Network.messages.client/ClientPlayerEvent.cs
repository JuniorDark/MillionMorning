using System;

namespace Code.Core.Network.messages.client;

public class ClientPlayerEvent : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 438;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientPlayerEvent(reader);
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

	private const int OPCODE = 438;

	private string eventTag;

	private string data;

	private sbyte targetType;

	private ClientPlayerEvent(MessageReader reader)
	{
		eventTag = reader.ReadString();
		data = reader.ReadString();
	}

	public ClientPlayerEvent(string eventTag, string data)
	{
		this.eventTag = eventTag;
		this.data = data;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(eventTag);
		num += MessageWriter.GetSize(data);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(438);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(eventTag);
		messageWriter.WriteString(data);
		return messageWriter.GetData();
	}
}
