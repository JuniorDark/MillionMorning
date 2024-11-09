using System;

namespace Code.Core.Network.messages.server;

public class ServerQuestAreaUnsubscribe : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 174;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerQuestAreaUnsubscribe(reader);
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

	private const int OPCODE = 174;

	private string fullAreaName;

	private ServerQuestAreaUnsubscribe(MessageReader reader)
	{
		fullAreaName = reader.ReadString();
	}

	public ServerQuestAreaUnsubscribe(string fullAreaName)
	{
		this.fullAreaName = fullAreaName;
	}

	public string getFullAreaName()
	{
		return fullAreaName;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(fullAreaName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(174);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullAreaName);
		return messageWriter.GetData();
	}
}
