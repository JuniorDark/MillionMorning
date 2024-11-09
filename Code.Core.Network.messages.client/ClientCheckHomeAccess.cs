namespace Code.Core.Network.messages.client;

public class ClientCheckHomeAccess : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 396;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientCheckHomeAccess(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 396;

	private int homeOwnerId;

	private ClientCheckHomeAccess(MessageReader reader)
	{
		homeOwnerId = reader.ReadInt32();
	}

	public ClientCheckHomeAccess(int homeOwnerId)
	{
		this.homeOwnerId = homeOwnerId;
	}

	public int getHomeOwnerId()
	{
		return homeOwnerId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(396);
		messageWriter.WriteInt32(homeOwnerId);
		return messageWriter.GetData();
	}
}
