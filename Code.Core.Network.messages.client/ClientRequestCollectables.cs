namespace Code.Core.Network.messages.client;

public class ClientRequestCollectables : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 129;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestCollectables(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 129;

	private ClientRequestCollectables(MessageReader reader)
	{
	}

	public ClientRequestCollectables()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(129);
		return messageWriter.GetData();
	}
}
