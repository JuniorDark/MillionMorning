namespace Code.Core.Network.messages.server;

public class ServerInventoryAmount : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 38;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInventoryAmount(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 6;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 38;

	private int itemID;

	private short amount;

	private ServerInventoryAmount(MessageReader reader)
	{
		itemID = reader.ReadInt32();
		amount = reader.ReadInt16();
	}

	public ServerInventoryAmount(int itemID, short amount)
	{
		this.itemID = itemID;
		this.amount = amount;
	}

	public int getItemID()
	{
		return itemID;
	}

	public short getAmount()
	{
		return amount;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(8);
		messageWriter.WriteOpCode(38);
		messageWriter.WriteInt32(itemID);
		messageWriter.WriteInt16(amount);
		return messageWriter.GetData();
	}
}
