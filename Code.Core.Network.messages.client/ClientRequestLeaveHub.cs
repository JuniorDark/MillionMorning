namespace Code.Core.Network.messages.client;

public class ClientRequestLeaveHub : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 288;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestLeaveHub(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 288;

	private ClientRequestLeaveHub(MessageReader reader)
	{
	}

	public ClientRequestLeaveHub()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(288);
		return messageWriter.GetData();
	}
}
