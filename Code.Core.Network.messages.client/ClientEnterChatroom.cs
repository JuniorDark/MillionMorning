namespace Code.Core.Network.messages.client;

public class ClientEnterChatroom : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 79;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientEnterChatroom(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 10;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 79;

	private long chatroomId;

	private short sitPointId;

	private ClientEnterChatroom(MessageReader reader)
	{
		chatroomId = reader.ReadInt64();
		sitPointId = reader.ReadInt16();
	}

	public ClientEnterChatroom(long chatroomId, short sitPointId)
	{
		this.chatroomId = chatroomId;
		this.sitPointId = sitPointId;
	}

	public long getChatroomId()
	{
		return chatroomId;
	}

	public short getSitPointId()
	{
		return sitPointId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(12);
		messageWriter.WriteOpCode(79);
		messageWriter.WriteInt64(chatroomId);
		messageWriter.WriteInt16(sitPointId);
		return messageWriter.GetData();
	}
}
