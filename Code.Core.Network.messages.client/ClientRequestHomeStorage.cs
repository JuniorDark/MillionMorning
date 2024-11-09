using System;

namespace Code.Core.Network.messages.client;

public class ClientRequestHomeStorage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 263;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestHomeStorage(reader);
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

	private const int OPCODE = 263;

	private string homeOwnerId;

	private ClientRequestHomeStorage(MessageReader reader)
	{
		homeOwnerId = reader.ReadString();
	}

	public ClientRequestHomeStorage(string homeOwnerId)
	{
		this.homeOwnerId = homeOwnerId;
	}

	public string getHomeOwnerId()
	{
		return homeOwnerId;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += MessageWriter.GetSize(homeOwnerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(263);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(homeOwnerId);
		return messageWriter.GetData();
	}
}
