namespace Code.Core.Network.messages.client;

public class ClientRequestStartScreenInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 299;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestStartScreenInfo(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 299;

	private ClientRequestStartScreenInfo(MessageReader reader)
	{
	}

	public ClientRequestStartScreenInfo()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(299);
		return messageWriter.GetData();
	}
}
