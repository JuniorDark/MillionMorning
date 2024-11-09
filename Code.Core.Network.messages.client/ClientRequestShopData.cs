namespace Code.Core.Network.messages.client;

public class ClientRequestShopData : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 69;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestShopData(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 0;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 69;

	private ClientRequestShopData(MessageReader reader)
	{
	}

	public ClientRequestShopData()
	{
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(2);
		messageWriter.WriteOpCode(69);
		return messageWriter.GetData();
	}
}
