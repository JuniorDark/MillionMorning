namespace Code.Core.Network.messages.client;

public class ClientActivateFurniture : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 268;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientActivateFurniture(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 268;

	private long furnitureId;

	private ClientActivateFurniture(MessageReader reader)
	{
		furnitureId = reader.ReadInt64();
	}

	public ClientActivateFurniture(long furnitureId)
	{
		this.furnitureId = furnitureId;
	}

	public long getFurnitureId()
	{
		return furnitureId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(268);
		messageWriter.WriteInt64(furnitureId);
		return messageWriter.GetData();
	}
}
