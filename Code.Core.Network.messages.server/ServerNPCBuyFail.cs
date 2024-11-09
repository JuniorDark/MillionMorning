namespace Code.Core.Network.messages.server;

public class ServerNPCBuyFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 20;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerNPCBuyFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 20;

	private int npcId;

	private int itemId;

	private ServerNPCBuyFail(MessageReader reader)
	{
		npcId = reader.ReadInt32();
		itemId = reader.ReadInt32();
	}

	public ServerNPCBuyFail(int npcId, int itemId)
	{
		this.npcId = npcId;
		this.itemId = itemId;
	}

	public int getNpcId()
	{
		return npcId;
	}

	public int getItemId()
	{
		return itemId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(20);
		messageWriter.WriteInt32(npcId);
		messageWriter.WriteInt32(itemId);
		return messageWriter.GetData();
	}
}
