using System;

namespace Code.Core.Network.messages.server;

public class ServerAchievementCompleted : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 145;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerAchievementCompleted(reader);
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

	private const int OPCODE = 145;

	private string playerId;

	private string achievement;

	private ServerAchievementCompleted(MessageReader reader)
	{
		playerId = reader.ReadString();
		achievement = reader.ReadString();
	}

	public ServerAchievementCompleted(string playerId, string achievement)
	{
		this.playerId = playerId;
		this.achievement = achievement;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public string getAchievement()
	{
		return achievement;
	}

	public byte[] GetData()
	{
		int num = 8;
		num += MessageWriter.GetSize(playerId);
		num += MessageWriter.GetSize(achievement);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(145);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(playerId);
		messageWriter.WriteString(achievement);
		return messageWriter.GetData();
	}
}
