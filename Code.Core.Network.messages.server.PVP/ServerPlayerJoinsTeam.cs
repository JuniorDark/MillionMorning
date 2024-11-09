using System;

namespace Code.Core.Network.messages.server.PVP;

public class ServerPlayerJoinsTeam : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 407;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerPlayerJoinsTeam(reader);
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

	private const int OPCODE = 407;

	private int teamId;

	private string playerId;

	private ServerPlayerJoinsTeam(MessageReader reader)
	{
		teamId = reader.ReadInt32();
		playerId = reader.ReadString();
	}

	public ServerPlayerJoinsTeam(int teamId, string playerId)
	{
		this.teamId = teamId;
		this.playerId = playerId;
	}

	public int GetTeamId()
	{
		return teamId;
	}

	public string GetPlayerId()
	{
		return playerId;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(407);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(teamId);
		messageWriter.WriteString(playerId);
		return messageWriter.GetData();
	}
}
