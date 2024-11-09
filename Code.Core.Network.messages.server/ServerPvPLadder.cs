using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerPvPLadder : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 387;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPvPLadder(reader);
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

	private const int OPCODE = 387;

	private IList<PvPLadderEntry> entries;

	private ServerPvPLadder(MessageReader reader)
	{
		entries = new List<PvPLadderEntry>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			entries.Add(new PvPLadderEntry(reader));
		}
	}

	public ServerPvPLadder(IList<PvPLadderEntry> entries)
	{
		this.entries = entries;
	}

	public IList<PvPLadderEntry> getEntries()
	{
		return entries;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (PvPLadderEntry entry in entries)
		{
			num += entry.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(387);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)entries.Count);
		foreach (PvPLadderEntry entry2 in entries)
		{
			entry2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
