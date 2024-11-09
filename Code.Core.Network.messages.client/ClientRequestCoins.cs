namespace Code.Core.Network.messages.client;

public class ClientRequestCoins : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 35;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestCoins(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 35;

	private ClientRequestCoins(MessageReader reader)
	{
	}

	public ClientRequestCoins()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(35);
		return messageWriter.GetData();
	}
}
