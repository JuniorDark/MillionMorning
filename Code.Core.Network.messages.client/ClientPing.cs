namespace Code.Core.Network.messages.client;

public class ClientPing : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 181;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientPing(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 181;

	private ClientPing(MessageReader reader)
	{
	}

	public ClientPing()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(181);
		return messageWriter.GetData();
	}
}
