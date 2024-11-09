using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLadderPage : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 381;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLadderPage(reader);
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

	private const int OPCODE = 381;

	private int pageNumber;

	private sbyte voteType;

	private IList<LadderEntry> entries;

	private ServerLadderPage(MessageReader reader)
	{
		pageNumber = reader.ReadInt32();
		voteType = reader.ReadInt8();
		entries = new List<LadderEntry>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			entries.Add(LadderEntry.Create(reader.ReadTypeCode(), reader));
		}
	}

	public ServerLadderPage(int pageNumber, sbyte voteType, IList<LadderEntry> entries)
	{
		this.pageNumber = pageNumber;
		this.voteType = voteType;
		this.entries = entries;
	}

	public int getPageNumber()
	{
		return pageNumber;
	}

	public sbyte getVoteType()
	{
		return voteType;
	}

	public IList<LadderEntry> getEntries()
	{
		return entries;
	}

	public byte[] GetData()
	{
		int num = 11;
		num += (short)(entries.Count * 2);
		foreach (LadderEntry entry in entries)
		{
			num += entry.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(381);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(pageNumber);
		messageWriter.WriteInt8(voteType);
		messageWriter.WriteInt16((short)entries.Count);
		foreach (LadderEntry entry2 in entries)
		{
			messageWriter.WriteTypeCode(entry2.GetTypeId());
			entry2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
