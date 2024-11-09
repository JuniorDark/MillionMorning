using System;

namespace Code.Core.Network.messages.server;

public class ServerActivateGameplayTrigger : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 156;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerActivateGameplayTrigger(reader);
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

	private const int OPCODE = 156;

	private int gameplayObjectId;

	private int triggerId;

	private string playerId;

	private ServerActivateGameplayTrigger(MessageReader reader)
	{
		gameplayObjectId = reader.ReadInt32();
		triggerId = reader.ReadInt32();
		playerId = reader.ReadString();
	}

	public ServerActivateGameplayTrigger(int gameplayObjectId, int triggerId, string playerId)
	{
		this.gameplayObjectId = gameplayObjectId;
		this.triggerId = triggerId;
		this.playerId = playerId;
	}

	public int getGameplayObjectId()
	{
		return gameplayObjectId;
	}

	public int getTriggerId()
	{
		return triggerId;
	}

	public string getPlayerId()
	{
		return playerId;
	}

	public byte[] GetData()
	{
		int num = 14;
		num += MessageWriter.GetSize(playerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(156);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteInt32(gameplayObjectId);
		messageWriter.WriteInt32(triggerId);
		messageWriter.WriteString(playerId);
		return messageWriter.GetData();
	}
}
