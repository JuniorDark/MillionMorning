using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerEquipUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 62;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEquipUpdate(reader);
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

	private const int OPCODE = 62;

	private string playerId;

	private IList<Item> items;

	private ServerEquipUpdate(MessageReader reader)
	{
		playerId = reader.ReadString();
		items = new List<Item>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			items.Add(Item.Create(reader.ReadTypeCode(), reader));
		}
	}

	public ServerEquipUpdate(string playerId, IList<Item> items)
	{
		this.playerId = playerId;
		this.items = items;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public IList<Item> getItems()
	{
		return items;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(playerId);
		num += (short)(items.Count * 2);
		foreach (Item item in items)
		{
			num += item.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(62);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt16((short)items.Count);
		foreach (Item item2 in items)
		{
			messageWriter.WriteTypeCode(item2.GetTypeId());
			item2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
