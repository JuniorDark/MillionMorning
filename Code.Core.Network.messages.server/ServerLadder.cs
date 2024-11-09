using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLadder : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 322;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLadder(reader);
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

	private const int OPCODE = 322;

	private IList<LadderEntry> entries;

	private ServerLadder(MessageReader reader)
	{
		entries = new List<LadderEntry>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			entries.Add(LadderEntry.Create(reader.ReadTypeCode(), reader));
		}
	}

	public ServerLadder(IList<LadderEntry> entries)
	{
		this.entries = entries;
	}

	public IList<LadderEntry> getEntries()
	{
		return entries;
	}

	public byte[] GetData()
	{
		int num = 6;
		num += (short)(entries.Count * 2);
		foreach (LadderEntry entry in entries)
		{
			num += entry.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(322);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)entries.Count);
		foreach (LadderEntry entry2 in entries)
		{
			messageWriter.WriteTypeCode(entry2.GetTypeId());
			entry2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
