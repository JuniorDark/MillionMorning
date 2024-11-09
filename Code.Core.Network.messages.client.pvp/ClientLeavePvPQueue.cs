namespace Code.Core.Network.messages.client.pvp;

public class ClientLeavePvPQueue : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 432;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientLeavePvPQueue(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 432;

	private ClientLeavePvPQueue(MessageReader reader)
	{
	}

	public ClientLeavePvPQueue()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(432);
		return messageWriter.GetData();
	}
}
