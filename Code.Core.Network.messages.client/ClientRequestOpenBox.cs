namespace Code.Core.Network.messages.client;

public class ClientRequestOpenBox : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 275;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestOpenBox(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 275;

	private int itemId;

	private ClientRequestOpenBox(MessageReader reader)
	{
		itemId = reader.ReadInt32();
	}

	public ClientRequestOpenBox(int itemId)
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
		messageWriter.WriteOpCode(275);
		messageWriter.WriteInt32(itemId);
		return messageWriter.GetData();
	}
}
