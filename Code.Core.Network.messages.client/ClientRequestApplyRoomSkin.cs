namespace Code.Core.Network.messages.client;

public class ClientRequestApplyRoomSkin : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 252;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientRequestApplyRoomSkin(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 16;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 252;

	private long skinItemId;

	private long roomId;

	private ClientRequestApplyRoomSkin(MessageReader reader)
	{
		skinItemId = reader.ReadInt64();
		roomId = reader.ReadInt64();
	}

	public ClientRequestApplyRoomSkin(long skinItemId, long roomId)
	{
		this.skinItemId = skinItemId;
		this.roomId = roomId;
	}

	public long getSkinItemId()
	{
		return skinItemId;
	}

	public long getRoomId()
	{
		return roomId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(18);
		messageWriter.WriteOpCode(252);
		messageWriter.WriteInt64(skinItemId);
		messageWriter.WriteInt64(roomId);
		return messageWriter.GetData();
	}
}
