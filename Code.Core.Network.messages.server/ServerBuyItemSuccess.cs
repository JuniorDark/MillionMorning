namespace Code.Core.Network.messages.server;

public class ServerBuyItemSuccess : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 73;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerBuyItemSuccess(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 73;

	private int itemId;

	private sbyte isGift;

	private ServerBuyItemSuccess(MessageReader reader)
	{
		itemId = reader.ReadInt32();
		isGift = reader.ReadInt8();
	}

	public ServerBuyItemSuccess(int itemId, sbyte isGift)
	{
		this.itemId = itemId;
		this.isGift = isGift;
	}

	public int getItemId()
	{
		return itemId;
	}

	public sbyte getIsGift()
	{
		return isGift;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(73);
		messageWriter.WriteInt32(itemId);
		messageWriter.WriteInt8(isGift);
		return messageWriter.GetData();
	}
}
