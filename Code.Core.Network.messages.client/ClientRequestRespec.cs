namespace Code.Core.Network.messages.client;

public class ClientRequestRespec : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 367;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestRespec(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 367;

	private int itemId;

	private ClientRequestRespec(MessageReader reader)
	{
		itemId = reader.ReadInt32();
	}

	public ClientRequestRespec(int itemId)
	{
		this.itemId = itemId;
	}

	public int getItemId()
	{
		return itemId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(367);
		messageWriter.WriteInt32(itemId);
		return messageWriter.GetData();
	}
}
