using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerHomeInstanceInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 242;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerHomeInstanceInfo(reader);
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

	private const int OPCODE = 242;

	private string homeOwnerId;

	private string homeName;

	private RoomInfo startRoomInfo;

	private sbyte hasDefaultEquipmentOnly;

	private ServerHomeInstanceInfo(MessageReader reader)
	{
		homeOwnerId = reader.ReadString();
		homeName = reader.ReadString();
		startRoomInfo = new RoomInfo(reader);
		hasDefaultEquipmentOnly = reader.ReadInt8();
	}

	public ServerHomeInstanceInfo(string homeOwnerId, string homeName, RoomInfo startRoomInfo, sbyte hasDefaultEquipmentOnly)
	{
		this.homeOwnerId = homeOwnerId;
		this.homeName = homeName;
		this.startRoomInfo = startRoomInfo;
		this.hasDefaultEquipmentOnly = hasDefaultEquipmentOnly;
	}

	public string getHomeOwnerId()
	{
		return homeOwnerId;
	}

	public string getHomeName()
	{
		return homeName;
	}

	public RoomInfo getStartRoomInfo()
	{
		return startRoomInfo;
	}

	public sbyte getHasDefaultEquipmentOnly()
	{
		return hasDefaultEquipmentOnly;
	}

	public byte[] GetData()
	{
		int num = 9;
		num += MessageWriter.GetSize(homeOwnerId);
		num += MessageWriter.GetSize(homeName);
		num += startRoomInfo.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(242);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(homeOwnerId);
		messageWriter.WriteString(homeName);
		startRoomInfo.Write(messageWriter);
		messageWriter.WriteInt8(hasDefaultEquipmentOnly);
		return messageWriter.GetData();
	}
}
