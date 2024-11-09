namespace Code.Core.Network.messages.client;

public class ClientRequestEquipArmor : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 127;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestEquipArmor(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 4;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 127;

	private int inventoryId;

	private ClientRequestEquipArmor(MessageReader reader)
	{
		inventoryId = reader.ReadInt32();
	}

	public ClientRequestEquipArmor(int inventoryId)
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
		messageWriter.WriteOpCode(127);
		messageWriter.WriteInt32(inventoryId);
		return messageWriter.GetData();
	}
}
