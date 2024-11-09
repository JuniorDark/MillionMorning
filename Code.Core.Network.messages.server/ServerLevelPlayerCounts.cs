using System;
using System.Collections.Generic;
using Code.Core.Network.types;

namespace Code.Core.Network.messages.server;

public class ServerLevelPlayerCounts : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 190;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerLevelPlayerCounts(reader);
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

	private const int OPCODE = 190;

	private IList<LevelPlayerCount> playerCounts;

	private ServerLevelPlayerCounts(MessageReader reader)
	{
		playerCounts = new List<LevelPlayerCount>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			playerCounts.Add(new LevelPlayerCount(reader));
		}
	}

	public ServerLevelPlayerCounts(IList<LevelPlayerCount> playerCounts)
	{
		this.playerCounts = playerCounts;
	}

	public IList<LevelPlayerCount> getPlayerCounts()
	{
		return playerCounts;
	}

	public byte[] GetData()
	{
		int num = 6;
		foreach (LevelPlayerCount playerCount in playerCounts)
		{
			num += playerCount.Size();
		}
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(190);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt16((short)playerCounts.Count);
		foreach (LevelPlayerCount playerCount2 in playerCounts)
		{
			playerCount2.Write(messageWriter);
		}
		return messageWriter.GetData();
	}
}
