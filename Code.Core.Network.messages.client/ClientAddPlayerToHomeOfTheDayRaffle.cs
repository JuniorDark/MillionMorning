namespace Code.Core.Network.messages.client;

public class ClientAddPlayerToHomeOfTheDayRaffle : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 401;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAddPlayerToHomeOfTheDayRaffle(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 401;

	private ClientAddPlayerToHomeOfTheDayRaffle(MessageReader reader)
	{
	}

	public ClientAddPlayerToHomeOfTheDayRaffle()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(401);
		return messageWriter.GetData();
	}
}
