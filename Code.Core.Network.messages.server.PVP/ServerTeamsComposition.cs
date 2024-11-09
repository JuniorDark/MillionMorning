using System;
using System.Collections.Generic;

namespace Code.Core.Network.messages.server.PVP;

public class ServerTeamsComposition : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 409;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerTeamsComposition(reader);
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

	private const int OPCODE = 409;

	private IList<NetworkTeam> teams;

	private ServerTeamsComposition(MessageReader reader)
	{
		teams = new List<NetworkTeam>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			teams.Add(new NetworkTeam(reader));
		}
	}

	public ServerTeamsComposition(IList<NetworkTeam> teams)
	{
		this.teams = teams;
	}

	public IList<NetworkTeam> GetTeams()
	{
		return teams;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (NetworkTeam team in teams)
		{
			num += team.size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(409);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)teams.Count);
		foreach (NetworkTeam team2 in teams)
		{
			team2.write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
