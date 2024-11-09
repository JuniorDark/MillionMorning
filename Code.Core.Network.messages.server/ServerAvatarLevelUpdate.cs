using System;

namespace Code.Core.Network.messages.server;

public class ServerAvatarLevelUpdate : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 308;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAvatarLevelUpdate(reader);
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

	private const int OPCODE = 308;

	private string playerId;

	private int avatarLevel;

	private int currentLevelExp;

	private int nextLevelExp;

	private string levelDescription;

	private sbyte availableClassLevels;

	private ServerAvatarLevelUpdate(MessageReader reader)
	{
		playerId = reader.ReadString();
		avatarLevel = reader.ReadInt32();
		currentLevelExp = reader.ReadInt32();
		nextLevelExp = reader.ReadInt32();
		levelDescription = reader.ReadString();
		availableClassLevels = reader.ReadInt8();
	}

	public ServerAvatarLevelUpdate(string playerId, int avatarLevel, int currentLevelExp, int nextLevelExp, string levelDescription, sbyte availableClassLevels)
	{
		this.playerId = playerId;
		this.avatarLevel = avatarLevel;
		this.currentLevelExp = currentLevelExp;
		this.nextLevelExp = nextLevelExp;
		this.levelDescription = levelDescription;
		this.availableClassLevels = availableClassLevels;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public int getAvatarLevel()
	{
		return avatarLevel;
	}

	public int getCurrentLevelExp()
	{
		return currentLevelExp;
	}

	public int getNextLevelExp()
	{
		return nextLevelExp;
	}

	public string getLevelDescription()
	{
		return levelDescription;
	}

	public sbyte getAvailableClassLevels()
	{
		return availableClassLevels;
	}

	public byte[] GetData()
	{
		int num = 21;
		num += MessageWriter.GetSize(playerId);
		num += MessageWriter.GetSize(levelDescription);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(308);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteInt32(avatarLevel);
		messageWriter.WriteInt32(currentLevelExp);
		messageWriter.WriteInt32(nextLevelExp);
		messageWriter.WriteString(levelDescription);
		messageWriter.WriteInt8(availableClassLevels);
		return messageWriter.GetData();
	}
}
