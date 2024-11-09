namespace Code.Core.Network.messages.client;

public class ClientRequestItemUnwield : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 36;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestItemUnwield(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 36;

	private ClientRequestItemUnwield(MessageReader reader)
	{
	}

	public ClientRequestItemUnwield()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(36);
		return messageWriter.GetData();
	}
}
