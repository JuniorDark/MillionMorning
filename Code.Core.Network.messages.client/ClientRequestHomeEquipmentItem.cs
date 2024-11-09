namespace Code.Core.Network.messages.client;

public class ClientRequestHomeEquipmentItem : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 262;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestHomeEquipmentItem(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 262;

	private long itemId;

	private ClientRequestHomeEquipmentItem(MessageReader reader)
	{
		itemId = reader.ReadInt64();
	}

	public ClientRequestHomeEquipmentItem(long itemId)
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
		messageWriter.WriteOpCode(262);
		messageWriter.WriteInt64(itemId);
		return messageWriter.GetData();
	}
}
