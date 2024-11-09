namespace Code.Core.Network.messages.server;

public class ServerEquipArmor : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 128;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEquipArmor(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 128;

	private int inventoryId;

	private ServerEquipArmor(MessageReader reader)
	{
		inventoryId = reader.ReadInt32();
	}

	public ServerEquipArmor(int inventoryId)
	{
		this.inventoryId = inventoryId;
	}

	public int getInventoryId()
	{
		return inventoryId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(6);
		messageWriter.WriteOpCode(128);
		messageWriter.WriteInt32(inventoryId);
		return messageWriter.GetData();
	}
}
