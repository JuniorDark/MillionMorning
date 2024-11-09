namespace Code.Core.Network.messages.client;

public class ClientNPCSellItem : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 18;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientNPCSellItem(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 10;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 18;

	private int npcId;

	private int itemId;

	private short amount;

	private ClientNPCSellItem(MessageReader reader)
	{
		npcId = reader.ReadInt32();
		itemId = reader.ReadInt32();
		amount = reader.ReadInt16();
	}

	public ClientNPCSellItem(int npcId, int itemId, short amount)
	{
		this.npcId = npcId;
		this.itemId = itemId;
		this.amount = amount;
	}

	public int getNpcId()
	{
		return npcId;
	}

	public int getItemId()
	{
		return itemId;
	}

	public short getAmount()
	{
		return amount;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(12);
		messageWriter.WriteOpCode(18);
		messageWriter.WriteInt32(npcId);
		messageWriter.WriteInt32(itemId);
		messageWriter.WriteInt16(amount);
		return messageWriter.GetData();
	}
}