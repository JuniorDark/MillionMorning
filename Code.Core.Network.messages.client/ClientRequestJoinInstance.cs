namespace Code.Core.Network.messages.client;

public class ClientRequestJoinInstance : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 245;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestJoinInstance(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 245;

	private ClientRequestJoinInstance(MessageReader reader)
	{
	}

	public ClientRequestJoinInstance()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(245);
		return messageWriter.GetData();
	}
}
