namespace Code.Core.Network.messages.client;

public class ClientRequestEat : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 205;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestEat(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 205;

	private ClientRequestEat(MessageReader reader)
	{
	}

	public ClientRequestEat()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(205);
		return messageWriter.GetData();
	}
}
