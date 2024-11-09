using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerHomeEquipment : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 248;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerHomeEquipment(reader);
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

	private const int OPCODE = 248;

	private sbyte isNew;

	private IList<HomeEquipment> items;

	private ServerHomeEquipment(MessageReader reader)
	{
		isNew = reader.ReadInt8();
		items = new List<HomeEquipment>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			items.Add((HomeEquipment)Item.Create(reader.ReadTypeCode(), reader));
		}
	}

	public ServerHomeEquipment(sbyte isNew, IList<HomeEquipment> items)
	{
		this.isNew = isNew;
		this.items = items;
	}

	public sbyte getIsNew()
	{
		return isNew;
	}

	public IList<HomeEquipment> getItems()
	{
		return items;
	}

	public byte[] GetData()
	{
		int num = 7;
		num += (short)(items.Count * 2);
		foreach (HomeEquipment item in items)
		{
			num += item.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(248);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt8(isNew);
		messageWriter.WriteInt16((short)items.Count);
		foreach (HomeEquipment item2 in items)
		{
			messageWriter.WriteTypeCode(item2.GetTypeId());
			item2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
