namespace Code.Core.Network.messages.client;

public class ClientRequestScoreBoard : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 316;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestScoreBoard(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 316;

	private ClientRequestScoreBoard(MessageReader reader)
	{
	}

	public ClientRequestScoreBoard()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(316);
		return messageWriter.GetData();
	}
}
