using System;

namespace Code.Core.Network.messages.server;

public class ServerEnterGameplayObject : IMessage
{
	public class Factory : IMessageFactory
	{
		public int GetOpCode()
		{
			return 101;
		}

		public IMessage CreateMessage(MessageReader reader)
		{
			return new ServerEnterGameplayObject(reader);
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

	private const int OPCODE = 101;

	private string PlayerId;

	private int gameplayObjectId;

	private ServerEnterGameplayObject(MessageReader reader)
	{
		PlayerId = reader.ReadString();
		gameplayObjectId = reader.ReadInt32();
	}

	public ServerEnterGameplayObject(string PlayerId, int gameplayObjectId)
	{
		this.PlayerId = PlayerId;
		this.gameplayObjectId = gameplayObjectId;
	}

	public string getPlayerId()
	{
		return PlayerId;
	}

	public int getGameplayObjectId()
	{
		return gameplayObjectId;
	}

	public byte[] GetData()
	{
		int num = 10;
		num += MessageWriter.GetSize(PlayerId);
		MessageWriter messageWriter = new MessageWriter(num);
		messageWriter.WriteOpCode(101);
		messageWriter.WriteInt16((short)(num - 4));
		messageWriter.WriteString(PlayerId);
		messageWriter.WriteInt32(gameplayObjectId);
		return messageWriter.GetData();
	}
}
