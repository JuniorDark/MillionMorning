using System;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerInventoryEntryUpgraded : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 312;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInventoryEntryUpgraded(reader);
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

	private const int OPCODE = 312;

	private int id;

	private Item newItem;

	private int newLevel;

	private TemplateReference nextLevel;

	private ServerInventoryEntryUpgraded(MessageReader reader)
	{
		id = reader.ReadInt32();
		newItem = Item.Create(reader.ReadTypeCode(), reader);
		newLevel = reader.ReadInt32();
		if (reader.ReadInt8() == 1)
		{
			nextLevel = new TemplateReference(reader);
		}
	}

	public ServerInventoryEntryUpgraded(int id, Item newItem, int newLevel, TemplateReference nextLevel)
	{
		this.id = id;
		this.newItem = newItem;
		this.newLevel = newLevel;
		this.nextLevel = nextLevel;
	}

	public int getId()
	{
		return id;
	}

	public Item getNewItem()
	{
		return newItem;
	}

	public int getNewLevel()
	{
		return newLevel;
	}

	public TemplateReference getNextLevel()
	{
		return nextLevel;
	}

	public byte[] GetData()
	{
		int num = 15;
		num += newItem.Size();
		if (nextLevel != null)
		{
			num += nextLevel.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(312);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(id);
		messageWriter.WriteTypeCode(newItem.GetTypeId());
		newItem.Write(messageWriter);
		messageWriter.WriteInt32(newLevel);
		if (nextLevel == null)
		{
			messageWriter.WriteInt8(0);
		}
		else
		{
			messageWriter.WriteInt8(1);
			nextLevel.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
