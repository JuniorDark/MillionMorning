namespace Code.Core.Network.messages.server;

public class ServerBuyItemFail : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 74;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerBuyItemFail(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 74;

	private int itemId;

	private sbyte failCode;

	private ServerBuyItemFail(MessageReader reader)
	{
		itemId = reader.ReadInt32();
		failCode = reader.ReadInt8();
	}

	public ServerBuyItemFail(int itemId, sbyte failCode)
	{
		this.itemId = itemId;
		this.failCode = failCode;
	}

	public int getItemId()
	{
		return itemId;
	}

	public sbyte getFailCode()
	{
		return failCode;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(74);
		messageWriter.WriteInt32(itemId);
		messageWriter.WriteInt8(failCode);
		return messageWriter.GetData();
	}
}
