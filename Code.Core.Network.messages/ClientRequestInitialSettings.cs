namespace Code.Core.Network.messages;

public class ClientRequestInitialSettings : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 434;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestInitialSettings(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 434;

	private ClientRequestInitialSettings(MessageReader reader)
	{
	}

	public ClientRequestInitialSettings()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(434);
		return messageWriter.GetData();
	}
}
