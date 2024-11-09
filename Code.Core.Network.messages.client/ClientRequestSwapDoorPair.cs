namespace Code.Core.Network.messages.client;

public class ClientRequestSwapDoorPair : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 398;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestSwapDoorPair(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 398;

	private long doorId;

	private ClientRequestSwapDoorPair(MessageReader reader)
	{
		doorId = reader.ReadInt64();
	}

	public ClientRequestSwapDoorPair(long doorId)
	{
		this.doorId = doorId;
	}

	public long getDoorId()
	{
		return doorId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(398);
		messageWriter.WriteInt64(doorId);
		return messageWriter.GetData();
	}
}
