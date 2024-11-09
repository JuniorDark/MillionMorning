namespace Code.Core.Network.messages.client;

public class ClientRequestPickupHomeDeliveryBox : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 301;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestPickupHomeDeliveryBox(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 301;

	private long itemId;

	private ClientRequestPickupHomeDeliveryBox(MessageReader reader)
	{
		itemId = reader.ReadInt64();
	}

	public ClientRequestPickupHomeDeliveryBox(long itemId)
	{
		this.itemId = itemId;
	}

	public long getItemId()
	{
		return itemId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(301);
		messageWriter.WriteInt64(itemId);
		return messageWriter.GetData();
	}
}
