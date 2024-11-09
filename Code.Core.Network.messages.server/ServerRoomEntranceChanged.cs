namespace Code.Core.Network.messages.server;

public class ServerRoomEntranceChanged : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 391;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRoomEntranceChanged(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 0;
			length = 24;
			return buffer.Remaining() >= length + 2;
		}
	}

	private const int OPCODE = 391;

	private long roomId;

	private long oldEntranceId;

	private long newEntranceId;

	private ServerRoomEntranceChanged(MessageReader reader)
	{
		roomId = reader.ReadInt64();
		oldEntranceId = reader.ReadInt64();
		newEntranceId = reader.ReadInt64();
	}

	public ServerRoomEntranceChanged(long roomId, long oldEntranceId, long newEntranceId)
	{
		this.roomId = roomId;
		this.oldEntranceId = oldEntranceId;
		this.newEntranceId = newEntranceId;
	}

	public long getRoomId()
	{
		return roomId;
	}

	public long getOldEntranceId()
	{
		return oldEntranceId;
	}

	public long getNewEntranceId()
	{
		return newEntranceId;
	}

	public byte[] GetData()
	{
		MessageWriter messageWriter = new MessageWriter(26);
		messageWriter.WriteOpCode(391);
		messageWriter.WriteInt64(roomId);
		messageWriter.WriteInt64(oldEntranceId);
		messageWriter.WriteInt64(newEntranceId);
		return messageWriter.GetData();
	}
}
