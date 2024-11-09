namespace Code.Core.Network.messages.client;

public class ClientRequestHomeOfTheDay : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 403;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestHomeOfTheDay(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 403;

	private ClientRequestHomeOfTheDay(MessageReader reader)
	{
	}

	public ClientRequestHomeOfTheDay()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(403);
		return messageWriter.GetData();
	}
}
