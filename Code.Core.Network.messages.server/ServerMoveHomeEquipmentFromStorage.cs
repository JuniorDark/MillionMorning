using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerMoveHomeEquipmentFromStorage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 258;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerMoveHomeEquipmentFromStorage(reader);
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

	private const int OPCODE = 258;

	private HomeEquipment equipment;

	private ServerMoveHomeEquipmentFromStorage(MessageReader reader)
	{
		equipment = (HomeEquipment)Item.Create(reader.ReadTypeCode(), reader);
	}

	public ServerMoveHomeEquipmentFromStorage(HomeEquipment equipment)
	{
		this.equipment = equipment;
	}

	public HomeEquipment getEquipment()
	{
		return equipment;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += equipment.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(258);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteTypeCode(equipment.GetTypeId());
		equipment.Write(messageWriter);
		return messageWriter.GetData();
	}
}
