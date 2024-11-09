namespace Code.Core.Network.messages.client;

public class ClientSetHasPlayed : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 374;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSetHasPlayed(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 374;

	private ClientSetHasPlayed(MessageReader reader)
	{
	}

	public ClientSetHasPlayed()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(374);
		return messageWriter.GetData();
	}
}
