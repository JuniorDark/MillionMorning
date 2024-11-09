namespace Code.Core.Network.messages.client;

public class ClientKeepalive : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 304;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientKeepalive(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 304;

	private ClientKeepalive(MessageReader reader)
	{
	}

	public ClientKeepalive()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(304);
		return messageWriter.GetData();
	}
}
