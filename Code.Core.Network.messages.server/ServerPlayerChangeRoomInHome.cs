using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerPlayerChangeRoomInHome : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 383;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerChangeRoomInHome(reader);
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

	private const int OPCODE = 383;

	private string playerId;

	private RoomInfo roomInfo;

	private long enteredThroughDoorId;

	private ServerPlayerChangeRoomInHome(MessageReader reader)
	{
		playerId = reader.ReadString();
		roomInfo = new RoomInfo(reader);
		enteredThroughDoorId = reader.ReadInt64();
	}

	public ServerPlayerChangeRoomInHome(string playerId, RoomInfo roomInfo, long enteredThroughDoorId)
	{
		this.playerId = playerId;
		this.roomInfo = roomInfo;
		this.enteredThroughDoorId = enteredThroughDoorId;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public RoomInfo getRoomInfo()
	{
		return roomInfo;
	}

	public long getEnteredThroughDoorId()
	{
		return enteredThroughDoorId;
	}

	public byte[] GetData()
	{
		int num = 14;
		num += MessageWriter.GetSize(playerId);
		num += roomInfo.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(383);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		roomInfo.Write(messageWriter);
		messageWriter.WriteInt64(enteredThroughDoorId);
		return messageWriter.GetData();
	}
}
