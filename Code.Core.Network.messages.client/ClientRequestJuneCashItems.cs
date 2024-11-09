namespace Code.Core.Network.messages.client;

public class ClientRequestJuneCashItems : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 428;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestJuneCashItems(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 428;

	private ClientRequestJuneCashItems(MessageReader reader)
	{
	}

	public ClientRequestJuneCashItems()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(428);
		return messageWriter.GetData();
	}
}
