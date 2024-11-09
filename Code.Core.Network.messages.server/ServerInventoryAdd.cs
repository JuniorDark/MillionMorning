using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerInventoryAdd : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 33;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerInventoryAdd(reader);
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

	private const int OPCODE = 33;

	private IList<InventoryEntry> entries;

	private sbyte isNewEntry;

	private sbyte showGameDialog;

	private ServerInventoryAdd(MessageReader reader)
	{
		entries = new List<InventoryEntry>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			entries.Add(InventoryEntry.Create(reader.ReadTypeCode(), reader));
		}
		isNewEntry = reader.ReadInt8();
		showGameDialog = reader.ReadInt8();
	}

	public ServerInventoryAdd(IList<InventoryEntry> entries, sbyte isNewEntry, sbyte showGameDialog)
	{
		this.entries = entries;
		this.isNewEntry = isNewEntry;
		this.showGameDialog = showGameDialog;
	}

	public IList<InventoryEntry> getEntries()
	{
		return entries;
	}

	public sbyte getIsNewEntry()
	{
		return isNewEntry;
	}

	public sbyte getShowGameDialog()
	{
		return showGameDialog;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += (short)(entries.Count * 2);
		foreach (InventoryEntry entry in entries)
		{
			num += entry.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(33);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)entries.Count);
		foreach (InventoryEntry entry2 in entries)
		{
			messageWriter.WriteTypeCode(entry2.GetTypeId());
			entry2.Write(messageWriter);
		}
		messageWriter.WriteInt8(isNewEntry);
		messageWriter.WriteInt8(showGameDialog);
		return messageWriter.GetData();
	}
}
