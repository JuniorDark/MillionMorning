namespace Code.Core.Network.messages.client;

public class ClientRequestNexusToken : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 375;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestNexusToken(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 375;

	private ClientRequestNexusToken(MessageReader reader)
	{
	}

	public ClientRequestNexusToken()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(375);
		return messageWriter.GetData();
	}
}
