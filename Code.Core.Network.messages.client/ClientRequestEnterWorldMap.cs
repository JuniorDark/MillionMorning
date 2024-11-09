namespace Code.Core.Network.messages.client;

public class ClientRequestEnterWorldMap : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 137;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestEnterWorldMap(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 137;

	private ClientRequestEnterWorldMap(MessageReader reader)
	{
	}

	public ClientRequestEnterWorldMap()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(137);
		return messageWriter.GetData();
	}
}
