using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerRecreateInventoryAndAmmo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 313;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerRecreateInventoryAndAmmo(reader);
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

	public const int OPCODE = 313;

	private IList<InventoryEntry> items;

	private IList<AmmoType> ammoTypes;

	private IList<string> hotkeys;

	private ServerRecreateInventoryAndAmmo(MessageReader reader)
	{
		items = new List<InventoryEntry>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			items.Add(InventoryEntry.Create(reader.ReadTypeCode(), reader));
		}
		ammoTypes = new List<AmmoType>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			ammoTypes.Add(new AmmoType(reader));
		}
		hotkeys = new List<string>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			hotkeys.Add(reader.ReadString());
		}
	}

	public ServerRecreateInventoryAndAmmo(IList<InventoryEntry> items, IList<AmmoType> ammoTypes, IList<string> hotkeys)
	{
		this.items = items;
		this.ammoTypes = ammoTypes;
		this.hotkeys = hotkeys;
	}

	public IList<InventoryEntry> getItems()
	{
		return items;
	}

	public IList<AmmoType> getAmmoTypes()
	{
		return ammoTypes;
	}

	public IList<string> getHotkeys()
	{
		return hotkeys;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += (short)(items.Count * 2);
		foreach (InventoryEntry item in items)
		{
			num += item.Size();
		}
		foreach (AmmoType ammoType in ammoTypes)
		{
			num += ammoType.Size();
		}
		num += (short)(2 * hotkeys.Count);
		foreach (string hotkey in hotkeys)
		{
			num += MessageWriter.GetSize(hotkey);
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(313);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)items.Count);
		foreach (InventoryEntry item2 in items)
		{
			messageWriter.WriteTypeCode(item2.GetTypeId());
			item2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)ammoTypes.Count);
		foreach (AmmoType ammoType2 in ammoTypes)
		{
			ammoType2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)hotkeys.Count);
		foreach (string hotkey2 in hotkeys)
		{
			messageWriter.WriteString(hotkey2);
		}
		return messageWriter.GetData();
	}
}
