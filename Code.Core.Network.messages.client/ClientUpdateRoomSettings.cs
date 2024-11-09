using System;

namespace Code.Core.Network.messages.client;

public class ClientUpdateRoomSettings : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 384;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ClientUpdateRoomSettings(reader);
		}

		public bool GetMessageSize(ByteBuffer buffer, out int length, out int lengthSize)
		{
			lengthSize = 2;
			if (buffer.Remaining() < lengthSize + 2)
			{
				length = 0;
				return false;
			}
			byte[] array = new byte[lengthSize];
			Array.Copy(buffer.Bytes, buffer.Pos + 2, array, 0, lengthSize);
			MessageReader messageReader = new MessageReader(array);
			length = messageReader.ReadInt16();
			return buffer.Remaining() >= length + lengthSize + 2;
		}
	}

	private const int OPCODE = 384;

	private long roomId;

	private string roomName;

	private sbyte accessLevel;

	private ClientUpdateRoomSettings(MessageReader reader)
	{
		roomId = reader.ReadInt64();
		roomName = reader.ReadString();
		accessLevel = reader.ReadInt8();
	}

	public ClientUpdateRoomSettings(long roomId, string roomName, sbyte accessLevel)
	{
		this.roomId = roomId;
		this.roomName = roomName;
		this.accessLevel = accessLevel;
	}

	public long getRoomId()
	{
		return roomId;
	}

	public string getRoomName()
	{
		return roomName;
	}

	public sbyte getAccessLevel()
	{
		return accessLevel;
	}

	public byte[] GetData()
	{
		int num = 15;
		num += MessageWriter.GetSize(roomName);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(384);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt64(roomId);
		messageWriter.WriteString(roomName);
		messageWriter.WriteInt8(accessLevel);
		return messageWriter.GetData();
	}
}
