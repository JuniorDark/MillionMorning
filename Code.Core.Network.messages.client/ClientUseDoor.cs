namespace Code.Core.Network.messages.client;

public class ClientUseDoor : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 382;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUseDoor(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 382;

	private long furnitureId;

	private ClientUseDoor(MessageReader reader)
	{
		furnitureId = reader.ReadInt64();
	}

	public ClientUseDoor(long furnitureId)
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
		messageWriter.WriteOpCode(382);
		messageWriter.WriteInt64(furnitureId);
		return messageWriter.GetData();
	}
}
