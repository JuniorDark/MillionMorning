using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerMoveHomeEquipmentFromOtherRoom : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 400;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerMoveHomeEquipmentFromOtherRoom(reader);
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

	private const int OPCODE = 400;

	private HomeEquipment equipment;

	private long oldRoomId;

	private ServerMoveHomeEquipmentFromOtherRoom(MessageReader reader)
	{
		equipment = (HomeEquipment)Item.Create(reader.ReadTypeCode(), reader);
		oldRoomId = reader.ReadInt64();
	}

	public ServerMoveHomeEquipmentFromOtherRoom(HomeEquipment equipment, long oldRoomId)
	{
		this.equipment = equipment;
		this.oldRoomId = oldRoomId;
	}

	public HomeEquipment getEquipment()
	{
		return equipment;
	}

	public long getOldRoomId()
	{
		return oldRoomId;
	}

	public byte[] GetData()
	{
		int num = 14;
		num += equipment.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(400);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteTypeCode(equipment.GetTypeId());
		equipment.Write(messageWriter);
		messageWriter.WriteInt64(oldRoomId);
		return messageWriter.GetData();
	}
}
