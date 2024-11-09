namespace Code.Core.Network.messages.client;

public class ClientLeaveChatroom : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 81;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientLeaveChatroom(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 81;

	private long chatroomId;

	private ClientLeaveChatroom(MessageReader reader)
	{
		chatroomId = reader.ReadInt64();
	}

	public ClientLeaveChatroom(long chatroomId)
	{
		this.chatroomId = chatroomId;
	}

	public long getChatroomId()
	{
		return chatroomId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(81);
		messageWriter.WriteInt64(chatroomId);
		return messageWriter.GetData();
	}
}
