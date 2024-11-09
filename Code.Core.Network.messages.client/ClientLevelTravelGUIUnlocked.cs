using System;

namespace Code.Core.Network.messages.client;

public class ClientLevelTravelGUIUnlocked : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 169;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientLevelTravelGUIUnlocked(reader);
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

	private const int OPCODE = 169;

	private string fullLevelName;

	private ClientLevelTravelGUIUnlocked(MessageReader reader)
	{
		fullLevelName = reader.ReadString();
	}

	public ClientLevelTravelGUIUnlocked(string fullLevelName)
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
		messageWriter.WriteOpCode(169);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(fullLevelName);
		return messageWriter.GetData();
	}
}
