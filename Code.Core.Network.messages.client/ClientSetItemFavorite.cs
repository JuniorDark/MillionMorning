namespace Code.Core.Network.messages.client;

public class ClientSetItemFavorite : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 230;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientSetItemFavorite(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 5;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 230;

	private int itemId;

	private sbyte favorite;

	private ClientSetItemFavorite(MessageReader reader)
	{
		itemId = reader.ReadInt32();
		favorite = reader.ReadInt8();
	}

	public ClientSetItemFavorite(int itemId, sbyte favorite)
	{
		this.itemId = itemId;
		this.favorite = favorite;
	}

	public int getItemId()
	{
		return itemId;
	}

	public sbyte getFavorite()
	{
		return favorite;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(7);
		messageWriter.WriteOpCode(230);
		messageWriter.WriteInt32(itemId);
		messageWriter.WriteInt8(favorite);
		return messageWriter.GetData();
	}
}
