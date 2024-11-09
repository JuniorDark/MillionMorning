namespace Code.Core.Network.messages.server;

public class ServerFurnitureActivated : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 269;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerFurnitureActivated(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 10;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 269;

	private long furnitureId;

	private short stateIndex;

	private ServerFurnitureActivated(MessageReader reader)
	{
		furnitureId = reader.ReadInt64();
		stateIndex = reader.ReadInt16();
	}

	public ServerFurnitureActivated(long furnitureId, short stateIndex)
	{
		this.furnitureId = furnitureId;
		this.stateIndex = stateIndex;
	}

	public long getFurnitureId()
	{
		return furnitureId;
	}

	public short getStateIndex()
	{
		return stateIndex;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(12);
		messageWriter.WriteOpCode(269);
		messageWriter.WriteInt64(furnitureId);
		messageWriter.WriteInt16(stateIndex);
		return messageWriter.GetData();
	}
}
