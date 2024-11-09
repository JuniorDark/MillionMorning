namespace Code.Core.Network.messages.client;

public class ClientRequestInventoryItemCheck : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 212;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestInventoryItemCheck(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 212;

	private int itemId;

	private ClientRequestInventoryItemCheck(MessageReader reader)
	{
		itemId = reader.ReadInt32();
	}

	public ClientRequestInventoryItemCheck(int itemId)
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
		messageWriter.WriteOpCode(212);
		messageWriter.WriteInt32(itemId);
		return messageWriter.GetData();
	}
}
