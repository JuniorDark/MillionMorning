namespace Code.Core.Network.messages.client;

public class ClientAlive : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 231;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientAlive(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 231;

	private ClientAlive(MessageReader reader)
	{
	}

	public ClientAlive()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(231);
		return messageWriter.GetData();
	}
}
