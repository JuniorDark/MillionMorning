namespace Code.Core.Network.messages.client;

public class ClientRequestCharbuilderData : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 76;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestCharbuilderData(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 76;

	private ClientRequestCharbuilderData(MessageReader reader)
	{
	}

	public ClientRequestCharbuilderData()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(76);
		return messageWriter.GetData();
	}
}
