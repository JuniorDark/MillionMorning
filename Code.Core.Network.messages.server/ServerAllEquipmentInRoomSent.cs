namespace Code.Core.Network.messages.server;

public class ServerAllEquipmentInRoomSent : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 249;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAllEquipmentInRoomSent(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 8;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 249;

	private long roomId;

	private ServerAllEquipmentInRoomSent(MessageReader reader)
	{
		roomId = reader.ReadInt64();
	}

	public ServerAllEquipmentInRoomSent(long roomId)
	{
		this.roomId = roomId;
	}

	public long getRoomId()
	{
		return roomId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(10);
		messageWriter.WriteOpCode(249);
		messageWriter.WriteInt64(roomId);
		return messageWriter.GetData();
	}
}
