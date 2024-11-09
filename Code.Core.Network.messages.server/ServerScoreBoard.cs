using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerScoreBoard : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 315;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerScoreBoard(reader);
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

	private const int OPCODE = 315;

	private IList<ScoreBoardEntry> entries;

	private IList<TeamScoreEntry> teamEntries;

	private sbyte matchHasEnded;

	private ServerScoreBoard(MessageReader reader)
	{
		entries = new List<ScoreBoardEntry>();
		teamEntries = new List<TeamScoreEntry>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			entries.Add(new ScoreBoardEntry(reader));
		}
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			teamEntries.Add(new TeamScoreEntry(reader));
		}
		matchHasEnded = reader.ReadInt8();
	}

	public ServerScoreBoard(IList<ScoreBoardEntry> entries, IList<TeamScoreEntry> teamEntries, bool matchHasEnded)
	{
		this.entries = entries;
		this.teamEntries = teamEntries;
		this.matchHasEnded = (sbyte)(matchHasEnded ? 1 : 0);
	}

	public IList<ScoreBoardEntry> getEntries()
	{
		return entries;
	}

	public IList<TeamScoreEntry> getTeamEntries()
	{
		return teamEntries;
	}

	public bool MatchHasEnded()
	{
		return matchHasEnded == 1;
	}

	public byte[] GetData()
	{
		int num = 9;
		foreach (ScoreBoardEntry entry in entries)
		{
			num += entry.Size();
		}
		foreach (TeamScoreEntry teamEntry in teamEntries)
		{
			num += teamEntry.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(315);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)entries.Count);
		foreach (ScoreBoardEntry entry2 in entries)
		{
			entry2.Write(messageWriter);
		}
		messageWriter.WriteInt16((short)teamEntries.Count);
		foreach (TeamScoreEntry teamEntry2 in teamEntries)
		{
			teamEntry2.Write(messageWriter);
		}
		messageWriter.WriteInt8(matchHasEnded);
		return messageWriter.GetData();
	}
}
