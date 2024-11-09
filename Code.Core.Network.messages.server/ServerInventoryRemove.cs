namespace Code.Core.Network.messages.server;

public class ServerInventoryRemove : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 41;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInventoryRemove(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 41;

	private int itemID;

	private ServerInventoryRemove(MessageReader reader)
	{
		itemID = reader.ReadInt32();
	}

	public ServerInventoryRemove(int itemID)
	{
		this.itemID = itemID;
	}

	public int getItemID()
	{
		return itemID;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(41);
		messageWriter.WriteInt32(itemID);
		return messageWriter.GetData();
	}
}
