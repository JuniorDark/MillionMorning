namespace Code.Core.Network.messages.client;

public class ClientRequestNotifications : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 208;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestNotifications(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 208;

	private ClientRequestNotifications(MessageReader reader)
	{
	}

	public ClientRequestNotifications()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(208);
		return messageWriter.GetData();
	}
}
