using System;

namespace Code.Core.Network.messages.server.PVP;

public class ServerPvPLevelInstanceInfo : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 410;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPvPLevelInstanceInfo(reader);
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

	private const int OPCODE = 410;

	private sbyte teamMode;

	private sbyte matchMode;

	private int nrOfRounds;

	private int maxRoundScore;

	private ServerPvPLevelInstanceInfo(MessageReader reader)
	{
		teamMode = reader.ReadInt8();
		matchMode = reader.ReadInt8();
		nrOfRounds = reader.ReadInt32();
		maxRoundScore = reader.ReadInt32();
	}

	public ServerPvPLevelInstanceInfo(bool isTeamMode, sbyte matchMode, int nrOfRounds, int maxRoundScore)
	{
		teamMode = (sbyte)(isTeamMode ? 1 : 0);
		this.matchMode = matchMode;
		this.nrOfRounds = nrOfRounds;
		this.maxRoundScore = maxRoundScore;
	}

	public bool isTeamMode()
	{
		return teamMode == 1;
	}

	public sbyte getMatchMode()
	{
		return matchMode;
	}

	public int getNrOfRounds()
	{
		return nrOfRounds;
	}

	public int getMaxRoundScore()
	{
		return maxRoundScore;
	}

	public byte[] GetData()
	{
		int num = 14;
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(410);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt8(teamMode);
		messageWriter.WriteInt8(matchMode);
		messageWriter.WriteInt32(nrOfRounds);
		messageWriter.WriteInt32(maxRoundScore);
		return messageWriter.GetData();
	}
}
