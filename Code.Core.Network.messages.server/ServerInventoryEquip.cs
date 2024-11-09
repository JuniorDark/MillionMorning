namespace Code.Core.Network.messages.server;

public class ServerInventoryEquip : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 39;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInventoryEquip(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 39;

	private int itemId;

	private ServerInventoryEquip(MessageReader reader)
	{
		itemId = reader.ReadInt32();
	}

	public ServerInventoryEquip(int itemId)
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
		messageWriter.WriteOpCode(39);
		messageWriter.WriteInt32(itemId);
		return messageWriter.GetData();
	}
}
