using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerApplyRoomSkin : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 253;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerApplyRoomSkin(reader);
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

	private const int OPCODE = 253;

	private HomeEquipment item;

	private ServerApplyRoomSkin(MessageReader reader)
	{
		item = (HomeEquipment)Item.Create(reader.ReadTypeCode(), reader);
	}

	public ServerApplyRoomSkin(HomeEquipment item)
	{
		this.item = item;
	}

	public HomeEquipment getItem()
	{
		return item;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += item.Size();
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(253);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteTypeCode(item.GetTypeId());
		item.Write(messageWriter);
		return messageWriter.GetData();
	}
}
