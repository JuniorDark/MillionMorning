namespace Code.Core.Network.messages.client;

public class ClientRequestEnterHub : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 287;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestEnterHub(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 287;

	private ClientRequestEnterHub(MessageReader reader)
	{
	}

	public ClientRequestEnterHub()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(287);
		return messageWriter.GetData();
	}
}
