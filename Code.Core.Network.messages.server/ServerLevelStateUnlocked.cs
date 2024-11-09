using System;

namespace Code.Core.Network.messages.server;

public class ServerLevelStateUnlocked : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 168;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLevelStateUnlocked(reader);
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

	private const int OPCODE = 168;

	private string fullLevelName;

	private ServerLevelStateUnlocked(MessageReader reader)
	{
		fullLevelName = reader.ReadString();
	}

	public ServerLevelStateUnlocked(string fullLevelName)
	{
		this.fullLevelName = fullLevelName;
	}

	public string getFullLevelName()
	{
		return fullLevelName;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(fullLevelName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(168);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullLevelName);
		return messageWriter.GetData();
	}
}
